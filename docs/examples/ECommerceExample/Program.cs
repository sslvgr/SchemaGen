using Microsoft.EntityFrameworkCore;
using SchemaGen.Core.Markdown.SchemaGen;
using SchemaGen.Core.Mermaid.SchemaGen;
using SchemaGen.Core.SqlDdl.SchemaGen;

// Create an e-commerce context for demonstration
using var context = new ECommerceContext();

Console.WriteLine("SchemaGen E-Commerce Example");
Console.WriteLine("============================");
Console.WriteLine();
Console.WriteLine("This example demonstrates using individual SchemaGen packages:");
Console.WriteLine("- SchemaGen.Core.Markdown");
Console.WriteLine("- SchemaGen.Core.Mermaid");
Console.WriteLine("- SchemaGen.Core.SqlDdl");
Console.WriteLine();

// Create output directory
Directory.CreateDirectory("output");

Console.WriteLine("Generating documentation...");
Console.WriteLine();

// 1. Generate Markdown documentation
Console.WriteLine("1. Generating Markdown documentation...");
var markdown = MarkdownSchemaGenerator.Generate(context);
var markdownPath = Path.Combine("output", "README.md");
File.WriteAllText(markdownPath, markdown);
Console.WriteLine($"   ✓ Saved to: {markdownPath}");

// 2. Generate Mermaid ERD diagram
Console.WriteLine("2. Generating Mermaid ERD diagram...");
var mermaidDiagram = MermaidErdGenerator.Generate(context);
var mermaidPath = Path.Combine("output", "erd.md");
File.WriteAllText(mermaidPath, mermaidDiagram);
Console.WriteLine($"   ✓ Saved to: {mermaidPath}");

// 3. Generate SQL DDL script
Console.WriteLine("3. Generating SQL DDL script...");
var sqlDdl = SqlDdlGenerator.Generate(context);
var sqlPath = Path.Combine("output", "schema.sql");
File.WriteAllText(sqlPath, sqlDdl);
Console.WriteLine($"   ✓ Saved to: {sqlPath}");

Console.WriteLine();
Console.WriteLine("✓ Documentation generation completed!");
Console.WriteLine();
Console.WriteLine("Generated files:");
Console.WriteLine($"  📄 {markdownPath}");
Console.WriteLine($"  📊 {mermaidPath}");
Console.WriteLine($"  🗃️ {sqlPath}");

// E-commerce domain model
public class ECommerceContext : DbContext
{
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ECommerceExample;Trusted_Connection=true;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Customer entity
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.SKU).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.SKU).IsUnique();
            
            // Foreign key to Category
            entity.HasOne(e => e.Category)
                  .WithMany(e => e.Products)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Order entity
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).HasMaxLength(20).IsRequired();
            entity.Property(e => e.OrderDate).IsRequired();
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            
            // Foreign key to Customer
            entity.HasOne(e => e.Customer)
                  .WithMany(e => e.Orders)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure OrderItem entity
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
            
            // Foreign key to Order
            entity.HasOne(e => e.Order)
                  .WithMany(e => e.OrderItems)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            // Foreign key to Product
            entity.HasOne(e => e.Product)
                  .WithMany(e => e.OrderItems)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Review entity
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Rating).IsRequired();
            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.ReviewDate).IsRequired();
            
            // Foreign key to Product
            entity.HasOne(e => e.Product)
                  .WithMany(e => e.Reviews)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            // Foreign key to Customer
            entity.HasOne(e => e.Customer)
                  .WithMany(e => e.Reviews)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    
    // Navigation properties
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string SKU { get; set; } = null!;
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
    
    // Navigation properties
    public Category Category { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = null!;
    public int CustomerId { get; set; }
    
    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    
    // Navigation properties
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

public class Review
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime ReviewDate { get; set; }
    public int ProductId { get; set; }
    public int CustomerId { get; set; }
    
    // Navigation properties
    public Product Product { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
}