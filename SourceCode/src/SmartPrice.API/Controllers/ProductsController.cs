using Microsoft.AspNetCore.Mvc;
using SmartPrice.Application.Interfaces;
using SmartPrice.Domain.Entities;

namespace SmartPrice.API.Controllers;

/// <summary>
/// Products management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductRepository productRepository,
        ILogger<ProductsController> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    /// <returns>List of all products</returns>
    /// <response code="200">Returns the list of products</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        _logger.LogInformation("Fetching all products");
        var products = await _productRepository.GetAllAsync();
        return Ok(products);
    }

    /// <summary>
    /// Get a specific product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>The requested product</returns>
    /// <response code="200">Returns the product</response>
    /// <response code="404">Product not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Product>> GetById(Guid id)
    {
        _logger.LogInformation("Fetching product with ID: {ProductId}", id);
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found", id);
            return NotFound(new { message = $"Product with ID {id} not found" });
        }

        return Ok(product);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    /// <param name="product">Product data</param>
    /// <returns>The created product</returns>
    /// <response code="201">Product created successfully</response>
    /// <response code="400">Invalid product data</response>
    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Product>> Create([FromBody] Product product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Creating new product: {ProductName}", product.Name);

        var exists = await _productRepository.ExistsAsync(product.Url);
        if (exists)
        {
            _logger.LogWarning("Product with URL {Url} already exists", product.Url);
            return BadRequest(new { message = "Product with this URL already exists" });
        }

        var createdProduct = await _productRepository.AddAsync(product);
        _logger.LogInformation("Product created with ID: {ProductId}", createdProduct.Id);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdProduct.Id },
            createdProduct);
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="product">Updated product data</param>
    /// <returns>No content</returns>
    /// <response code="204">Product updated successfully</response>
    /// <response code="400">Invalid data</response>
    /// <response code="404">Product not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] Product product)
    {
        if (id != product.Id)
        {
            return BadRequest(new { message = "ID mismatch" });
        }

        var existing = await _productRepository.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found for update", id);
            return NotFound(new { message = $"Product with ID {id} not found" });
        }

        _logger.LogInformation("Updating product with ID: {ProductId}", id);
        await _productRepository.UpdateAsync(product);

        return NoContent();
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>No content</returns>
    /// <response code="204">Product deleted successfully</response>
    /// <response code="404">Product not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found for deletion", id);
            return NotFound(new { message = $"Product with ID {id} not found" });
        }

        _logger.LogInformation("Deleting product with ID: {ProductId}", id);
        await _productRepository.DeleteAsync(id);

        return NoContent();
    }
}
