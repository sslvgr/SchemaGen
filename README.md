# SchemaGen

A modern .NET library and CLI tool for generating documentation and diagrams from Entity Framework database schemas.

## Overview

SchemaGen extracts database schema information from Entity Framework models and generates:
- **Markdown documentation** - Comprehensive database schema documentation
- **Mermaid diagrams** - Entity relationship diagrams for visualization
- **SQL DDL scripts** - Data Definition Language scripts for database creation

## Packages

This repository contains the following NuGet packages:

- **SchemaGen.Core** - Meta-package containing all core functionality
- **SchemaGen.Core.Markdown** - Markdown documentation generation
- **SchemaGen.Core.Mermaid** - Mermaid diagram generation
- **SchemaGen.Core.SqlDdl** - SQL DDL script generation
- **SchemaGen.Tool** - Command-line interface tool

## Installation

### Using the CLI Tool

```bash
dotnet tool install --global SchemaGen.Tool
```

### Using the Libraries

```xml
<PackageReference Include="SchemaGen.Core" Version="1.0.0" />
```

## Quick Start

### Command Line Usage

```bash
# Generate all outputs for a DbContext
schemagen --context MyDbContext --output ./docs

# Generate only Markdown documentation
schemagen --context MyDbContext --markdown --output ./docs/schema.md

# Generate only Mermaid diagram
schemagen --context MyDbContext --mermaid --output ./docs/schema.mmd
```

### Library Usage

```csharp
using SchemaGen.Core.Markdown;
using SchemaGen.Core.Mermaid;
using SchemaGen.Core.SqlDdl;

// Generate Markdown documentation
var markdownGenerator = new MarkdownGenerator();
var markdown = markdownGenerator.Generate(dbContext);

// Generate Mermaid diagram
var mermaidGenerator = new MermaidGenerator();
var diagram = mermaidGenerator.Generate(dbContext);

// Generate SQL DDL
var sqlGenerator = new SqlDdlGenerator();
var ddl = sqlGenerator.Generate(dbContext);
```

## Requirements

- .NET 8.0 or later
- Entity Framework Core 8.0 or later

## Building from Source

```bash
git clone https://github.com/Teez-Technologies/SchemaGen.git
cd SchemaGen
dotnet build
dotnet test
```

## Contributing

Contributions are welcome! Please read our contributing guidelines and submit pull requests to the main branch.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For questions and support, please open an issue on GitHub.