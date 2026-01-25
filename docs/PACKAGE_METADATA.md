# Package Metadata Configuration

This document describes the package metadata configuration for all SchemaGen packages.

## Configured Metadata

All packages include the following metadata configured in `Directory.Build.props`:

### Basic Information
- **Authors**: Teez Technologies
- **Company**: Teez Technologies
- **Product**: SchemaGen
- **Copyright**: Copyright © Teez Technologies
- **License**: MIT (PackageLicenseExpression)

### Repository Information
- **Repository URL**: https://github.com/Teez-Technologies/SchemaGen
- **Repository Type**: git
- **Project URL**: https://github.com/Teez-Technologies/SchemaGen
- **README**: README.md (included in packages)

### Package-Specific Metadata

Each package includes specific descriptions and tags:

#### SchemaGen.Core (Meta-package)
- **Description**: Meta-package for SchemaGen core functionality including Markdown, Mermaid, and SQL DDL generation capabilities.
- **Tags**: schema;generation;meta-package;entity-framework;database

#### SchemaGen.Core.Markdown
- **Description**: Markdown generation functionality for SchemaGen - converts database schemas to Markdown documentation.
- **Tags**: schema;generation;markdown;documentation;entity-framework;database

#### SchemaGen.Core.Mermaid
- **Description**: Mermaid diagram generation functionality for SchemaGen - creates entity relationship diagrams from database schemas.
- **Tags**: schema;generation;mermaid;diagrams;erd;entity-framework;database

#### SchemaGen.Core.SqlDdl
- **Description**: SQL DDL generation functionality for SchemaGen - generates SQL Data Definition Language scripts from Entity Framework models.
- **Tags**: schema;generation;sql;ddl;scripts;entity-framework;database

#### SchemaGen.Tool
- **Description**: Command-line tool for SchemaGen - generates documentation and diagrams from Entity Framework database schemas.
- **Tags**: schema;generation;cli;tool;command-line;entity-framework;database

## Package Icon

**Note**: Package icons are not currently configured due to NuGet's requirement for PNG/JPG format. 
To add package icons in the future:

1. Create a 64x64 or 128x128 PNG icon file
2. Place it in the `assets/` directory
3. Add the following to `Directory.Build.props`:
   ```xml
   <PackageIcon>package-icon.png</PackageIcon>
   ```
4. Include the icon file in packages:
   ```xml
   <None Include="$(MSBuildThisFileDirectory)assets/package-icon.png" Pack="true" PackagePath="\" />
   ```

## Quality Features

All packages are configured with:
- **Deterministic builds**: Enabled for reproducible outputs
- **Symbol packages**: Generated (.snupkg) for debugging
- **SourceLink**: Configured for source code navigation
- **Multi-targeting**: .NET 8, 9, and 10 support
- **Central package management**: Consistent dependency versions