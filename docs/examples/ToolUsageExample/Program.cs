using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

Console.WriteLine("SchemaGen Tool Usage Example");
Console.WriteLine("============================");
Console.WriteLine();
Console.WriteLine("This project demonstrates how to set up a project for use with the SchemaGen CLI tool.");
Console.WriteLine("The actual schema generation is done using the 'schemagen' command-line tool.");
Console.WriteLine();
Console.WriteLine("To use the SchemaGen tool with this project:");
Console.WriteLine();
Console.WriteLine("1. Install the SchemaGen tool globally:");
Console.WriteLine("   dotnet tool install --global SchemaGen.Tool");
Console.WriteLine();
Console.WriteLine("2. Build this project:");
Console.WriteLine("   dotnet build");
Console.WriteLine();
Console.WriteLine("3. Run the tool:");
Console.WriteLine("   schemagen --project ToolUsageExample.csproj --output docs/database");
Console.WriteLine();
Console.WriteLine("4. Or use the assembly directly:");
Console.WriteLine("   schemagen --assembly bin/Debug/net8.0/ToolUsageExample.dll --output docs/database");
Console.WriteLine();
Console.WriteLine("See the README.md file for complete usage instructions and examples.");

// Simple inventory management system for demonstration
public class InventoryContext : DbContext
{
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;
    public DbSet<StockMovement> StockMovements { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=InventorySystem;Trusted_Connection=true;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.SKU).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.SKU).IsUnique();
            
            // Foreign key to Category
            entity.HasOne(e => e.Category)
                  .WithMany(e => e.Products)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            // Foreign key to Supplier
            entity.HasOne(e => e.Supplier)
                  .WithMany(e => e.Products)
                  .HasForeignKey(e => e.SupplierId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure Supplier entity
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.ContactEmail).HasMaxLength(255);
            entity.Property(e => e.ContactPhone).HasMaxLength(20);
        });

        // Configure StockMovement entity
        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MovementType).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.MovementDate).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(500);
            
            // Foreign key to Product
            entity.HasOne(e => e.Product)
                  .WithMany(e => e.StockMovements)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

// Design-time factory for EF Core tooling and SchemaGen
public class InventoryContextFactory : IDesignTimeDbContextFactory<InventoryContext>
{
    public InventoryContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<InventoryContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=InventorySystem;Trusted_Connection=true;");
        
        return new InventoryContext();
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string SKU { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
    
    // Navigation properties
    public Category Category { get; set; } = null!;
    public Supplier Supplier { get; set; } = null!;
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    
    // Navigation properties
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Address { get; set; }
    
    // Navigation properties
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class StockMovement
{
    public int Id { get; set; }
    public string MovementType { get; set; } = null!; // "IN", "OUT", "ADJUSTMENT"
    public int Quantity { get; set; }
    public DateTime MovementDate { get; set; }
    public string? Notes { get; set; }
    public int ProductId { get; set; }
    
    // Navigation properties
    public Product Product { get; set; } = null!;
}