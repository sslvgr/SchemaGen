# Basic SQL DDL Example

This example demonstrates how to use `SchemaGen.Core.SqlDdl` to generate SQL DDL (Data Definition Language) scripts from an Entity Framework DbContext.

## What This Example Shows

- How to reference the `SchemaGen.Core.SqlDdl` package
- How to create an Entity Framework DbContext with SQL Server provider
- How to use `SqlDdlGenerator.Generate()` to create DDL scripts
- Sample output showing CREATE TABLE statements with constraints and indexes

## Domain Model

The example uses an e-commerce system with five entities:
- **Product** - Represents products with name, price, and SKU
- **Category** - Represents product categories
- **Customer** - Represents customers with contact information
- **Order** - Represents customer orders with order details
- **OrderItem** - Represents individual items within orders

## Key Features Demonstrated

- **Table Creation**: Complete CREATE TABLE statements for all entities
- **Data Types**: Proper SQL Server data type mapping (decimal, varchar, etc.)
- **Constraints**: Primary keys, foreign keys, and unique constraints
- **Indexes**: Unique indexes for business keys (SKU, email, etc.)
- **Relationships**: Foreign key constraints with proper cascade behavior

## Running the Example

```bash
cd BasicSqlDdlExample
dotnet restore
dotnet build
dotnet run
```

## Expected Output

The example will generate a `schema.sql` file in the `output/` directory containing complete SQL DDL statements.

## Sample Generated DDL

```sql
-- =====================================================
-- Database Schema DDL
-- DbContext: ECommerceContext
-- Generated: 2024-01-15 10:30:00 UTC
-- =====================================================

CREATE TABLE [Categories] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
);

CREATE TABLE [Customers] (
    [Id] int NOT NULL IDENTITY,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [Email] nvarchar(255) NOT NULL,
    [Phone] nvarchar(20) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
);

CREATE TABLE [Products] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [Price] decimal(18,2) NOT NULL,
    [SKU] nvarchar(50) NOT NULL,
    [StockQuantity] int NOT NULL,
    [CategoryId] int NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Orders] (
    [Id] int NOT NULL IDENTITY,
    [OrderNumber] nvarchar(20) NOT NULL,
    [OrderDate] datetime2 NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [CustomerId] int NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [OrderItems] (
    [Id] int NOT NULL IDENTITY,
    [Quantity] int NOT NULL,
    [UnitPrice] decimal(18,2) NOT NULL,
    [TotalPrice] decimal(18,2) NOT NULL,
    [OrderId] int NOT NULL,
    [ProductId] int NOT NULL,
    CONSTRAINT [PK_OrderItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION
);

CREATE UNIQUE INDEX [IX_Categories_Name] ON [Categories] ([Name]);
CREATE UNIQUE INDEX [IX_Customers_Email] ON [Customers] ([Email]);
CREATE INDEX [IX_Orders_CustomerId] ON [Orders] ([CustomerId]);
CREATE UNIQUE INDEX [IX_Orders_OrderNumber] ON [Orders] ([OrderNumber]);
CREATE INDEX [IX_OrderItems_OrderId] ON [OrderItems] ([OrderId]);
CREATE INDEX [IX_OrderItems_ProductId] ON [OrderItems] ([ProductId]);
CREATE INDEX [IX_Products_CategoryId] ON [Products] ([CategoryId]);
CREATE UNIQUE INDEX [IX_Products_SKU] ON [Products] ([SKU]);
```

## Database Provider Support

This example uses SQL Server as the database provider. The generated DDL will contain SQL Server-specific syntax including:

- `IDENTITY` columns for auto-incrementing primary keys
- `nvarchar` data types for Unicode strings
- `decimal(18,2)` for precise decimal values
- `datetime2` for date/time values
- SQL Server-specific constraint syntax

## Connection String Note

The example uses a connection string for DDL generation purposes only. The actual database connection is not established during DDL generation - Entity Framework uses the provider configuration to determine the appropriate SQL syntax.

## Using with Other Providers

To generate DDL for other database providers, simply change the provider in `OnConfiguring`:

```csharp
// PostgreSQL
optionsBuilder.UseNpgsql("Host=localhost;Database=ecommerce;Username=user;Password=pass");

// SQLite
optionsBuilder.UseSqlite("Data Source=ecommerce.db");

// MySQL
optionsBuilder.UseMySql("Server=localhost;Database=ecommerce;User=user;Password=pass;", 
    ServerVersion.AutoDetect("connection-string"));
```

## Package Reference

This example uses a project reference to the local SchemaGen.Core.SqlDdl project. In a real application, you would reference the NuGet package:

```xml
<PackageReference Include="SchemaGen.Core.SqlDdl" Version="1.0.0" />
```