# SchemaGen Examples

This directory contains example projects demonstrating how to use the SchemaGen packages to generate database schema documentation.

## Examples Overview

### 1. Basic Usage Examples
- **[BasicMarkdownExample](./BasicMarkdownExample/)** - Simple example showing Markdown documentation generation
- **[BasicMermaidExample](./BasicMermaidExample/)** - Simple example showing Mermaid ERD diagram generation  
- **[BasicSqlDdlExample](./BasicSqlDdlExample/)** - Simple example showing SQL DDL script generation

### 2. Complete Integration Examples
- **[BloggingExample](./BloggingExample/)** - Complete example using all SchemaGen packages with a blogging domain model
- **[ECommerceExample](./ECommerceExample/)** - Advanced example with complex relationships in an e-commerce domain

### 3. Tool Usage Examples
- **[ToolUsageExample](./ToolUsageExample/)** - Examples showing how to use the SchemaGen CLI tool

## Quick Start

Each example includes:
- A sample Entity Framework DbContext with domain models
- Project configuration showing package references
- Code demonstrating how to use each SchemaGen package
- Generated output samples
- Build and run instructions

## Prerequisites

- .NET 8.0 or later
- Entity Framework Core 8.0 or later
- SchemaGen packages (available from GitHub Packages)

## Running the Examples

1. Navigate to any example directory
2. Restore packages: `dotnet restore`
3. Build the project: `dotnet build`
4. Run the example: `dotnet run`

The examples will generate documentation in the `output/` directory within each example project.

## Package Installation

To use SchemaGen packages in your own projects, add the GitHub Packages source and install the packages:

```bash
# Add GitHub Packages source (replace YOUR_TOKEN with a GitHub personal access token)
dotnet nuget add source https://nuget.pkg.github.com/sslvgr/index.json \
  --name "GitHub Packages" \
  --username YOUR_USERNAME \
  --password YOUR_TOKEN \
  --store-password-in-clear-text

# Install packages
dotnet add package SchemaGen.Core
# Or install individual packages:
# dotnet add package SchemaGen.Core.Markdown
# dotnet add package SchemaGen.Core.Mermaid
# dotnet add package SchemaGen.Core.SqlDdl
```

## Documentation

For complete documentation, see the main [README.md](../../README.md) in the repository root.
