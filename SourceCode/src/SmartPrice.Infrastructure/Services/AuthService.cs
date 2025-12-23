using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SmartPrice.Application.DTOs.Auth;
using SmartPrice.Application.Interfaces;
using SmartPrice.Domain.Entities;
using SmartPrice.Domain.Enums;
using SmartPrice.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SmartPrice.Infrastructure.Services;

/// <summary>
/// سرویس احراز هویت
/// </summary>
public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken ct = default)
    {
        try
        {
            var user = await _context.AdminUsers
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive, ct);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt for username: {Username}", request.Username);
                return null;
            }

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7"));
            user.LastLoginAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("User {Username} logged in successfully", user.Username);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(
                    int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60") / 60),
                User = MapToAdminUserDto(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        try
        {
            var user = await _context.AdminUsers
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.IsActive, ct);

            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                _logger.LogWarning("Invalid or expired refresh token");
                return null;
            }

            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7"));

            await _context.SaveChangesAsync(ct);

            return new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(
                    int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60") / 60),
                User = MapToAdminUserDto(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> RegisterAdminAsync(
        string username,
        string password,
        string email,
        string fullName,
        AdminRole role,
        CancellationToken ct = default)
    {
        try
        {
            if (await _context.AdminUsers.AnyAsync(u => u.Username == username, ct))
            {
                _logger.LogWarning("Username already exists: {Username}", username);
                return false;
            }

            var admin = new AdminUser
            {
                Id = Guid.NewGuid(),
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Email = email,
                FullName = fullName,
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.AdminUsers.AddAsync(admin, ct);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("New admin registered: {Username}", username);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering admin");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> LogoutAsync(Guid userId, CancellationToken ct = default)
    {
        try
        {
            var user = await _context.AdminUsers.FindAsync(new object[] { userId }, cancellationToken: ct);
            if (user == null)
                return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("User {UserId} logged out", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> ChangePasswordAsync(
        Guid userId,
        string oldPassword,
        string newPassword,
        CancellationToken ct = default)
    {
        try
        {
            var user = await _context.AdminUsers.FindAsync(new object[] { userId }, cancellationToken: ct);
            if (user == null || !BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
            {
                _logger.LogWarning("Failed password change attempt for user: {UserId}", userId);
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Password changed for user: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            throw;
        }
    }

    /// <summary>
    /// ایجاد Access Token
    /// </summary>
    private string GenerateAccessToken(AdminUser user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? 
                throw new InvalidOperationException("Jwt:SecretKey not configured"))
        );
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "SmartPrice",
            audience: _configuration["Jwt:Audience"] ?? "SmartPriceUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// ایجاد Refresh Token
    /// </summary>
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// نگاشت AdminUser به AdminUserDto
    /// </summary>
    private AdminUserDto MapToAdminUserDto(AdminUser user)
    {
        return new AdminUserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString()
        };
    }
}
