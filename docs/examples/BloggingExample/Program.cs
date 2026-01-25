using Microsoft.EntityFrameworkCore;
using SchemaGen.Core.Markdown.SchemaGen;
using SchemaGen.Core.Mermaid.SchemaGen;
using SchemaGen.Core.SqlDdl.SchemaGen;

// Create a comprehensive blogging platform context
using var context = new BloggingPlatformContext();

Console.WriteLine("SchemaGen Complete Example - Blogging Platform");
Console.WriteLine("==============================================");
Console.WriteLine();
Console.WriteLine("This example demonstrates using all SchemaGen packages together:");
Console.WriteLine("- SchemaGen.Core.Markdown for documentation");
Console.WriteLine("- SchemaGen.Core.Mermaid for ERD diagrams");
Console.WriteLine("- SchemaGen.Core.SqlDdl for DDL scripts");
Console.WriteLine();

// Create output directory
Directory.CreateDirectory("output");

Console.WriteLine("Generating schema documentation...");
Console.WriteLine();

// 1. Generate Markdown documentation
Console.WriteLine("1. Generating Markdown documentation...");
var markdown = MarkdownSchemaGenerator.Generate(context);
var markdownPath = Path.Combine("output", "schema-documentation.md");
File.WriteAllText(markdownPath, markdown);
Console.WriteLine($"   ✓ Saved to: {markdownPath}");

// 2. Generate Mermaid ERD diagram
Console.WriteLine("2. Generating Mermaid ERD diagram...");
var mermaidDiagram = MermaidErdGenerator.Generate(context);
var mermaidPath = Path.Combine("output", "erd-diagram.md");
File.WriteAllText(mermaidPath, mermaidDiagram);
Console.WriteLine($"   ✓ Saved to: {mermaidPath}");

// 3. Generate SQL DDL script
Console.WriteLine("3. Generating SQL DDL script...");
var sqlDdl = SqlDdlGenerator.Generate(context);
var sqlPath = Path.Combine("output", "database-schema.sql");
File.WriteAllText(sqlPath, sqlDdl);
Console.WriteLine($"   ✓ Saved to: {sqlPath}");

Console.WriteLine();
Console.WriteLine("4. Generating combined documentation...");

// 4. Create a combined documentation file
var combinedDoc = CreateCombinedDocumentation(markdown, mermaidDiagram, sqlDdl);
var combinedPath = Path.Combine("output", "complete-documentation.md");
File.WriteAllText(combinedPath, combinedDoc);
Console.WriteLine($"   ✓ Saved to: {combinedPath}");

Console.WriteLine();
Console.WriteLine("==============================================");
Console.WriteLine("✓ Complete schema documentation generated!");
Console.WriteLine("==============================================");
Console.WriteLine();
Console.WriteLine("Generated files:");
Console.WriteLine($"  📄 {markdownPath} - Detailed table documentation");
Console.WriteLine($"  📊 {mermaidPath} - Entity relationship diagram");
Console.WriteLine($"  🗃️ {sqlPath} - Database creation script");
Console.WriteLine($"  📚 {combinedPath} - Combined documentation");
Console.WriteLine();
Console.WriteLine("Next steps:");
Console.WriteLine("  1. Review the generated documentation");
Console.WriteLine("  2. Use the SQL script to create your database");
Console.WriteLine("  3. Include the Markdown files in your project documentation");
Console.WriteLine("  4. View the Mermaid diagram in GitHub/GitLab or mermaid.live");

static string CreateCombinedDocumentation(string markdown, string mermaidDiagram, string sqlDdl)
{
    return $"""
# Blogging Platform - Complete Database Documentation

This document provides comprehensive documentation for the Blogging Platform database schema, including detailed table descriptions, entity relationship diagrams, and SQL DDL scripts.

## Table of Contents

1. [Overview](#overview)
2. [Entity Relationship Diagram](#entity-relationship-diagram)
3. [Detailed Table Documentation](#detailed-table-documentation)
4. [Database Creation Script](#database-creation-script)

## Overview

The Blogging Platform database supports a full-featured blogging system with the following key entities:

- **Users** - Platform users (authors and readers)
- **Blogs** - Individual blogs owned by users
- **Posts** - Blog posts with content and metadata
- **Comments** - User comments on posts
- **Tags** - Categorization tags for posts
- **Categories** - Hierarchical content categories

## Entity Relationship Diagram

The following diagram shows the relationships between all entities in the system:

{mermaidDiagram}

## Detailed Table Documentation

{markdown}

## Database Creation Script

The following SQL script can be used to create the complete database schema:

```sql
{sqlDdl}
```

---

*This documentation was generated automatically using SchemaGen packages.*
""";
}

// Comprehensive blogging platform domain model
public class BloggingPlatformContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Blog> Blogs { get; set; } = null!;
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=BloggingPlatform;Trusted_Connection=true;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Bio).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configure Blog entity
        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Slug).HasMaxLength(250).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(e => e.Slug).IsUnique();
            
            // Foreign key to User (owner)
            entity.HasOne(e => e.Owner)
                  .WithMany(e => e.Blogs)
                  .HasForeignKey(e => e.OwnerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Post entity
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(300).IsRequired();
            entity.Property(e => e.Slug).HasMaxLength(350).IsRequired();
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Excerpt).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(e => new { e.BlogId, e.Slug }).IsUnique();
            
            // Foreign key to Blog
            entity.HasOne(e => e.Blog)
                  .WithMany(e => e.Posts)
                  .HasForeignKey(e => e.BlogId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            // Foreign key to User (author)
            entity.HasOne(e => e.Author)
                  .WithMany(e => e.Posts)
                  .HasForeignKey(e => e.AuthorId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            // Foreign key to Category
            entity.HasOne(e => e.Category)
                  .WithMany(e => e.Posts)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.SetNull);
                  
            // Many-to-many relationship with Tags
            entity.HasMany(e => e.Tags)
                  .WithMany(e => e.Posts)
                  .UsingEntity("PostTags");
        });

        // Configure Comment entity
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).HasMaxLength(2000).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            // Foreign key to Post
            entity.HasOne(e => e.Post)
                  .WithMany(e => e.Comments)
                  .HasForeignKey(e => e.PostId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            // Foreign key to User (author)
            entity.HasOne(e => e.Author)
                  .WithMany(e => e.Comments)
                  .HasForeignKey(e => e.AuthorId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            // Self-referencing foreign key for replies
            entity.HasOne(e => e.ParentComment)
                  .WithMany(e => e.Replies)
                  .HasForeignKey(e => e.ParentCommentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Tag entity
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Slug).HasMaxLength(60).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.Slug).IsUnique();
        });

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Slug).HasMaxLength(120).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.Slug).IsUnique();
            
            // Self-referencing foreign key for hierarchy
            entity.HasOne(e => e.ParentCategory)
                  .WithMany(e => e.SubCategories)
                  .HasForeignKey(e => e.ParentCategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

public class Blog
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Slug { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public int OwnerId { get; set; }
    
    // Navigation properties
    public User Owner { get; set; } = null!;
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Excerpt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public bool IsPublished { get; set; }
    public int ViewCount { get; set; }
    public int BlogId { get; set; }
    public int AuthorId { get; set; }
    public int? CategoryId { get; set; }
    
    // Navigation properties
    public Blog Blog { get; set; } = null!;
    public User Author { get; set; } = null!;
    public Category? Category { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsApproved { get; set; }
    public int PostId { get; set; }
    public int AuthorId { get; set; }
    public int? ParentCommentId { get; set; }
    
    // Navigation properties
    public Post Post { get; set; } = null!;
    public User Author { get; set; } = null!;
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public string? Color { get; set; }
    
    // Navigation properties
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    
    // Navigation properties
    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}