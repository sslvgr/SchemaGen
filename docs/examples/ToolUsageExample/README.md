# SchemaGen CLI Tool Usage Example

This example demonstrates how to use the SchemaGen command-line tool (`schemagen`) to generate database schema documentation from an Entity Framework project.

## What This Example Shows

- **Design-Time Factory**: How to implement `IDesignTimeDbContextFactory<T>` for tool compatibility
- **CLI Tool Usage**: Various ways to invoke the SchemaGen tool
- **Project Structure**: Proper project setup for schema generation
- **Output Management**: Organizing generated documentation files

## Prerequisites

1. **Install the SchemaGen Tool**:
   ```bash
   dotnet tool install --global SchemaGen.Tool
   ```

2. **Verify Installation**:
   ```bash
   schemagen --help
   ```

## Domain Model

This example uses a simple inventory management system with:
- **Product** - Items in inventory with SKU, price, and stock levels
- **Category** - Product categories for organization
- **Supplier** - Product suppliers with contact information
- **StockMovement** - Inventory movements (in, out, adjustments)

## Key Implementation Details

### Design-Time Factory

The `InventoryContextFactory` class implements `IDesignTimeDbContextFactory<InventoryContext>`:

```csharp
public class InventoryContextFactory : IDesignTimeDbContextFactory<InventoryContext>
{
    public InventoryContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<InventoryContext>();
        optionsBuilder.UseSqlServer("connection-string-here");
        
        return new InventoryContext();
    }
}
```

This factory is required for:
- Entity Framework migrations
- SchemaGen tool discovery
- Design-time tooling support

## Usage Examples

### 1. Basic Usage with Project File

```bash
# Build and generate from project file
schemagen --project ToolUsageExample.csproj --output docs/database
```

This command:
- Builds the project automatically
- Discovers all DbContext factories
- Generates documentation for all contexts
- Outputs files to `docs/database/` directory

### 2. Using Pre-built Assembly

```bash
# Build first
dotnet build

# Generate from assembly
schemagen --assembly bin/Debug/net8.0/ToolUsageExample.dll --output docs/database
```

### 3. Targeting Specific Context

```bash
# Generate for specific context only
schemagen --project ToolUsageExample.csproj --context InventoryContext --output docs/database
```

### 4. List Available Contexts

```bash
# Discover available contexts
schemagen --project ToolUsageExample.csproj --list-contexts
```

Expected output:
```
Available contexts (via design-time factories):
- ToolUsageExample.InventoryContext
```

### 5. Advanced Configuration

```bash
# Specify build configuration and target framework
schemagen --project ToolUsageExample.csproj \
          --configuration Release \
          --tfm net8.0 \
          --context InventoryContext \
          --output docs/database
```

### 6. Passing Arguments to Factory

```bash
# Pass additional arguments to the CreateDbContext method
schemagen --project ToolUsageExample.csproj --output docs/database -- --environment Production
```

### 7. Debug Mode

```bash
# Enable debug output to troubleshoot discovery issues
schemagen --project ToolUsageExample.csproj --debug --output docs/database
```

## Generated Output Structure

After running the tool, you'll find the following structure:

```
docs/database/
└── inventory/
    ├── README.md          # Markdown documentation
    ├── diagram.md         # Mermaid ERD diagram
    └── ddl.sql           # SQL DDL script
```

The folder name (`inventory`) is derived from the DbContext name (`InventoryContext` → `inventory`).

## Generated Files

### README.md
Complete Markdown documentation including:
- Database overview and statistics
- Detailed table documentation
- Column definitions with types and constraints
- Relationship information
- Index definitions

### diagram.md
Mermaid ERD diagram showing:
- All entities and their attributes
- Relationship cardinalities
- Primary and foreign key indicators

### ddl.sql
SQL DDL script containing:
- CREATE TABLE statements
- Primary and foreign key constraints
- Index definitions
- SQL Server-specific syntax

## Integration with Build Process

### MSBuild Integration

Add to your `.csproj` file:

```xml
<Target Name="GenerateSchemaDoc" AfterTargets="Build">
  <Exec Command="schemagen --assembly $(OutputPath)$(AssemblyName).dll --output docs/database" 
        ContinueOnError="false" />
</Target>
```

### CI/CD Integration

Example GitHub Actions workflow:

```yaml
name: Generate Schema Documentation

on:
  push:
    branches: [ main ]
    paths: [ '**/*.cs', '**/*.csproj' ]

jobs:
  generate-docs:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
        
    - name: Install SchemaGen Tool
      run: dotnet tool install --global SchemaGen.Tool
      
    - name: Generate Documentation
      run: schemagen --project ToolUsageExample.csproj --output docs/database
      
    - name: Commit Documentation
      run: |
        git config --local user.email "action@github.com"
        git config --local user.name "GitHub Action"
        git add docs/database/
        git commit -m "Update schema documentation" || exit 0
        git push
```

## Troubleshooting

### Common Issues

1. **No contexts found**:
   - Ensure you have a class implementing `IDesignTimeDbContextFactory<T>`
   - Check that the project builds successfully
   - Use `--debug` flag to see discovery details

2. **Build failures**:
   - Verify all NuGet packages are restored
   - Check that the target framework is supported
   - Ensure connection strings are valid (even if not used)

3. **Assembly loading errors**:
   - Make sure all dependencies are in the output directory
   - Try using `--project` instead of `--assembly`
   - Check for version conflicts in dependencies

### Debug Output

Use the `--debug` flag to see detailed information:

```bash
schemagen --project ToolUsageExample.csproj --debug --output docs/database
```

This shows:
- Loaded assemblies
- Discovered factories
- Type loading errors
- Assembly resolution details

## Best Practices

1. **Always use design-time factories** for production applications
2. **Include the tool in your CI/CD pipeline** to keep documentation current
3. **Version control the generated documentation** to track schema changes
4. **Use meaningful output directories** that match your project structure
5. **Test the tool locally** before integrating into automated processes

## Package Requirements

This example requires:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.*" />
```

The Design package is required for the `IDesignTimeDbContextFactory<T>` interface.