-- Create AdminUsers table
CREATE TABLE IF NOT EXISTS "AdminUsers" (
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "Username" varchar(50) NOT NULL UNIQUE,
    "PasswordHash" text NOT NULL,
    "Email" varchar(200) NOT NULL,
    "FullName" varchar(200) NOT NULL,
    "Role" integer NOT NULL DEFAULT 0,
    "IsActive" boolean NOT NULL DEFAULT true,
    "LastLoginAt" timestamp with time zone,
    "RefreshToken" text,
    "RefreshTokenExpiryTime" timestamp with time zone,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT NOW(),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT NOW()
);

-- Create AuditLogs table
CREATE TABLE IF NOT EXISTS "AuditLogs" (
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "AdminUserId" uuid NOT NULL REFERENCES "AdminUsers"("Id"),
    "Action" varchar(100) NOT NULL,
    "EntityType" varchar(100),
    "EntityId" varchar(100),
    "OldValues" text,
    "NewValues" text,
    "IpAddress" varchar(50),
    "UserAgent" text,
    "Timestamp" timestamp with time zone NOT NULL DEFAULT NOW()
);

-- Create SystemMetrics table
CREATE TABLE IF NOT EXISTS "SystemMetrics" (
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "MetricType" integer NOT NULL,
    "MetricName" varchar(100) NOT NULL,
    "Value" double precision NOT NULL,
    "Details" text,
    "RecordedAt" timestamp with time zone NOT NULL DEFAULT NOW()
);

-- Insert admin user (password: Admin@123)
INSERT INTO "AdminUsers" ("Id", "Username", "PasswordHash", "Email", "FullName", "Role", "IsActive", "CreatedAt", "UpdatedAt") 
VALUES (
    gen_random_uuid(), 
    'admin', 
    '$2a$11$K7rXpKqLvH1hYW5.VvE9/.0X8jG4oTHfKbZsKZqVqcYq0bLXYvLVe', 
    'admin@smartprice.ir', 
    'System Administrator', 
    0, 
    true, 
    NOW(), 
    NOW()
) ON CONFLICT ("Username") DO NOTHING;
