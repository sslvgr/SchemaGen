# Basic Markdown Example

This example demonstrates how to use `SchemaGen.Core.Markdown` to generate Markdown documentation from an Entity Framework DbContext.

## What This Example Shows

- How to reference the `SchemaGen.Core.Markdown` package
- How to create a simple Entity Framework DbContext with relationships
- How to use `MarkdownSchemaGenerator.Generate()` to create documentation
- Sample output showing tables, columns, relationships, and constraints

## Domain Model

The example uses a simple blogging domain with three entities:
- **Blog** - Represents a blog with title and description
- **Post** - Represents blog posts with title, content, and timestamps
- **Author** - Represents post authors with name and email

## Key Features Demonstrated

- **Table Documentation**: Complete table structure with columns, types, and constraints
- **Relationship Mapping**: Foreign key relationships between entities
- **Index Documentation**: Unique indexes and constraints
- **Metadata Generation**: Automatic generation of table statistics and navigation

## Running the Example

```bash
cd BasicMarkdownExample
dotnet restore
dotnet build
dotnet run
```

## Expected Output

The example will generate a `schema.md` file in the `output/` directory containing:

- Database overview and statistics
- Table of contents with links
- Detailed documentation for each table including:
  - Column definitions with types and constraints
  - Primary key information
  - Foreign key relationships
  - Index definitions
  - Navigation properties

## Sample Generated Content

```markdown
# Database Schema

**Generated:** 2024-01-15 10:30:00 UTC

**DbContext:** `BlogContext`

## Database Information

- **Schema:** public (PostgreSQL)

## Statistics

- **Total Tables:** 3
- **Total Foreign Keys:** 2
- **Total Indexes:** 3

## Table of Contents

- [Authors](#authors)
- [Blogs](#blogs)
- [Posts](#posts)

## Tables

### Authors

**Schema:** `public`

**CLR Type:** `Author`

#### Columns

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| `Id` | `int` | No | `` | PK, OnAdd |
| `Name` | `nvarchar(100)` | No | `` |  |
| `Email` | `nvarchar(255)` | No | `` |  |

#### Primary Key

- **PK_Authors**: `Id`

#### Indexes

- **IX_Authors_Email**
  - Columns: `Email`
  - Type: Unique

#### Relationships

- **Posts** → `Posts` (One-to-Many)

---
```

## Package Reference

This example uses a project reference to the local SchemaGen.Core.Markdown project. In a real application, you would reference the NuGet package:

```xml
<PackageReference Include="SchemaGen.Core.Markdown" Version="1.0.0" />
```