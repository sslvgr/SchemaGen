using Microsoft.EntityFrameworkCore;
using SchemaGen.Core.Mermaid.SchemaGen;

// Create a library management context for demonstration
using var context = new LibraryContext();

Console.WriteLine("SchemaGen.Core.Mermaid - Basic Example");
Console.WriteLine("======================================");
Console.WriteLine();
Console.WriteLine("Note: This example uses SQL Server provider for schema generation.");
Console.WriteLine("No actual database connection is established.");
Console.WriteLine();

// Generate Mermaid ERD diagram
var mermaidDiagram = MermaidErdGenerator.Generate(context);

// Create output directory
Directory.CreateDirectory("output");

// Write to file
var outputPath = Path.Combine("output", "erd-diagram.md");
File.WriteAllText(outputPath, mermaidDiagram);

Console.WriteLine($"✓ Mermaid ERD diagram generated: {outputPath}");
Console.WriteLine();
Console.WriteLine("Generated diagram:");
Console.WriteLine("------------------");
Console.WriteLine(mermaidDiagram);
Console.WriteLine();
Console.WriteLine("To view the diagram:");
Console.WriteLine("1. Copy the content to a Mermaid-compatible viewer");
Console.WriteLine("2. Use GitHub/GitLab markdown preview");
Console.WriteLine("3. Use VS Code with Mermaid extension");
Console.WriteLine("4. Visit https://mermaid.live/ and paste the content");

// Library management domain model
public class LibraryContext : DbContext
{
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Member> Members { get; set; } = null!;
    public DbSet<Loan> Loans { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Use SQL Server with a connection string for schema generation
        // Note: This connection string is used only for schema generation, not actual database operations
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=LibraryExample;Trusted_Connection=true;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Book entity
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(300).IsRequired();
            entity.Property(e => e.ISBN).HasMaxLength(13).IsRequired();
            entity.Property(e => e.PublishedYear).IsRequired();
            entity.HasIndex(e => e.ISBN).IsUnique();
            
            // Many-to-many relationship with Authors
            entity.HasMany(e => e.Authors)
                  .WithMany(e => e.Books)
                  .UsingEntity("BookAuthors");
                  
            // Foreign key to Category
            entity.HasOne(e => e.Category)
                  .WithMany(e => e.Books)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Author entity
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Biography).HasMaxLength(1000);
        });

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure Member entity
        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.MembershipNumber).HasMaxLength(20).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.MembershipNumber).IsUnique();
        });

        // Configure Loan entity
        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LoanDate).IsRequired();
            entity.Property(e => e.DueDate).IsRequired();
            
            // Foreign key to Book
            entity.HasOne(e => e.Book)
                  .WithMany(e => e.Loans)
                  .HasForeignKey(e => e.BookId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            // Foreign key to Member
            entity.HasOne(e => e.Member)
                  .WithMany(e => e.Loans)
                  .HasForeignKey(e => e.MemberId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string ISBN { get; set; } = null!;
    public int PublishedYear { get; set; }
    public int CategoryId { get; set; }
    
    // Navigation properties
    public Category Category { get; set; } = null!;
    public ICollection<Author> Authors { get; set; } = new List<Author>();
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Biography { get; set; }
    
    // Navigation properties
    public ICollection<Book> Books { get; set; } = new List<Book>();
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    
    // Navigation properties
    public ICollection<Book> Books { get; set; } = new List<Book>();
}

public class Member
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string MembershipNumber { get; set; } = null!;
    public DateTime JoinDate { get; set; }
    
    // Navigation properties
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}

public class Loan
{
    public int Id { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    
    // Navigation properties
    public Book Book { get; set; } = null!;
    public Member Member { get; set; } = null!;
}