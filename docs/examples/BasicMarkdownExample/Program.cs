using Microsoft.EntityFrameworkCore;
using SchemaGen.Core.Markdown.SchemaGen;

// Create a simple blog context for demonstration
using var context = new BlogContext();

Console.WriteLine("SchemaGen.Core.Markdown - Basic Example");
Console.WriteLine("========================================");
Console.WriteLine();
Console.WriteLine("Note: This example uses SQL Server provider for schema generation.");
Console.WriteLine("No actual database connection is established.");
Console.WriteLine();

// Generate Markdown documentation
var markdown = MarkdownSchemaGenerator.Generate(context);

// Create output directory
Directory.CreateDirectory("output");

// Write to file
var outputPath = Path.Combine("output", "schema.md");
File.WriteAllText(outputPath, markdown);

Console.WriteLine($"✓ Markdown documentation generated: {outputPath}");
Console.WriteLine();
Console.WriteLine("Generated content preview:");
Console.WriteLine("-------------------------");
Console.WriteLine(markdown.Substring(0, Math.Min(500, markdown.Length)));
if (markdown.Length > 500)
{
    Console.WriteLine("...");
    Console.WriteLine($"(Full content written to {outputPath})");
}

// Simple blog domain model
public class BlogContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; } = null!;
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<Author> Authors { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Use SQL Server with a connection string for schema generation
        // Note: This connection string is used only for schema generation, not actual database operations
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=BlogExample;Trusted_Connection=true;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Blog entity
        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Title).IsUnique();
        });

        // Configure Post entity
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(300).IsRequired();
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Foreign key to Blog
            entity.HasOne(e => e.Blog)
                  .WithMany(e => e.Posts)
                  .HasForeignKey(e => e.BlogId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            // Foreign key to Author
            entity.HasOne(e => e.Author)
                  .WithMany(e => e.Posts)
                  .HasForeignKey(e => e.AuthorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Author entity
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
        });
    }
}

public class Blog
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public int BlogId { get; set; }
    public int AuthorId { get; set; }
    
    // Navigation properties
    public Blog Blog { get; set; } = null!;
    public Author Author { get; set; } = null!;
}

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    
    // Navigation properties
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}