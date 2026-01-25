# Requirements Document

## Introduction

This specification defines the requirements for extracting SchemaGen projects from the existing `Teez-Technologies/b2c-backend` solution and restructuring them as a standalone NuGet package repository with modern .NET best practices. The goal is to create a maintainable, distributable set of NuGet packages that follow industry standards for versioning, packaging, and CI/CD automation.

## Glossary

- **Source_Repository**: The existing `Teez-Technologies/b2c-backend` repository containing the original projects
- **Target_Repository**: The new standalone `SchemaGen` repository to be created at `C:\src\SchemaGen`
- **Package_Manager**: NuGet package management system for .NET
- **CI_Pipeline**: Continuous Integration pipeline using GitHub Actions
- **Multi_Targeting**: Supporting multiple .NET framework versions in a single package
- **Meta_Package**: A NuGet package that contains no code but references other packages as dependencies
- **Symbols_Package**: A NuGet package containing debugging symbols and source code for enhanced debugging experience
- **Deterministic_Build**: A build process that produces identical outputs given identical inputs
- **GitHub_Packages**: GitHub's private NuGet package registry service

## Requirements

### Requirement 1: Repository Extraction and Setup

**User Story:** As a developer, I want to extract SchemaGen projects from the existing solution into a new standalone repository, so that I can maintain and distribute them independently.

#### Acceptance Criteria

1. WHEN extracting projects from the source repository, THE System SHALL create a new git repository at `C:\src\SchemaGen`
2. WHEN copying project files, THE System SHALL preserve the existing code structure and functionality from `src/Teez.SchemaGen.Tool/Teez.SchemaGen.Tool.csproj`
3. WHEN copying project files, THE System SHALL preserve the existing code structure and functionality from `src/Teez.SchemaGen.Core/Teez.SchemaGen.Core.csproj`
4. WHEN setting up the new repository, THE System SHALL initialize it with proper git configuration and initial commit
5. WHEN extracting dependencies, THE System SHALL identify and include all required NuGet package references

### Requirement 2: Package Restructuring and Naming

**User Story:** As a package consumer, I want clearly named and logically organized packages, so that I can easily understand and use the appropriate components.

#### Acceptance Criteria

1. WHEN renaming packages, THE System SHALL rename `Teez.SchemaGen.Core` to `SchemaGen.Core`
2. WHEN renaming packages, THE System SHALL rename `Teez.SchemaGen.Tool` to `SchemaGen.Tool`
3. WHEN splitting core functionality, THE System SHALL create `SchemaGen.Core.Markdown` package for Markdown-specific functionality
4. WHEN splitting core functionality, THE System SHALL create `SchemaGen.Core.Mermaid` package for Mermaid-specific functionality
5. WHEN splitting core functionality, THE System SHALL create `SchemaGen.Core.SqlDdl` package for SQL DDL generation functionality
6. WHEN creating the meta-package, THE System SHALL configure `SchemaGen.Core` as a meta-package that references `SchemaGen.Core.Markdown`, `SchemaGen.Core.Mermaid`, and `SchemaGen.Core.SqlDdl`
7. WHEN organizing packages, THE System SHALL ensure clean separation of concerns between different package types

### Requirement 3: NuGet Package Configuration

**User Story:** As a package maintainer, I want properly configured NuGet packages with complete metadata, so that they can be published and consumed professionally.

#### Acceptance Criteria

1. WHEN configuring package metadata, THE System SHALL set the license to MIT for all packages
2. WHEN configuring package registry, THE System SHALL configure packages for GitHub Packages as a private registry
3. WHEN setting package properties, THE System SHALL include proper package description, authors, and project URL metadata
4. WHEN configuring package icons, THE System SHALL include appropriate package icon and repository information
5. WHEN setting up package dependencies, THE System SHALL configure proper inter-package dependencies within the solution

### Requirement 4: Multi-Targeting Framework Support

**User Story:** As a developer using different .NET versions, I want packages that support multiple framework versions, so that I can use them regardless of my target framework.

#### Acceptance Criteria

1. WHEN configuring target frameworks, THE System SHALL support .NET 8 through .NET 10 frameworks
2. WHEN building packages, THE System SHALL produce separate assemblies for each target framework
3. WHEN resolving dependencies, THE System SHALL ensure compatibility across all supported framework versions
4. WHEN packaging, THE System SHALL include appropriate framework-specific assemblies in the NuGet package structure

### Requirement 5: Centralized Version Management

**User Story:** As a maintainer, I want centralized version management across all packages, so that I can maintain consistent versioning without manual updates to individual projects.

#### Acceptance Criteria

1. WHEN setting up version management, THE System SHALL create a `Directory.Build.props` file in the repository root
2. WHEN configuring versioning, THE System SHALL define version properties centrally in `Directory.Build.props`
3. WHEN building packages, THE System SHALL apply the centralized version to all projects automatically
4. WHEN updating versions, THE System SHALL require changes only to the central configuration file
5. WHEN versioning packages, THE System SHALL use semantic versioning (SemVer) format

### Requirement 6: Dependency Management

**User Story:** As a package consumer, I want packages with up-to-date and properly managed dependencies, so that I get the latest security fixes and features.

#### Acceptance Criteria

1. WHEN configuring Entity Framework dependencies, THE System SHALL float EF Core to the latest patch version within the major version
2. WHEN managing package dependencies, THE System SHALL use central package management via `Directory.Packages.props`
3. WHEN resolving dependencies, THE System SHALL ensure all packages use consistent dependency versions
4. WHEN updating dependencies, THE System SHALL maintain compatibility across all target frameworks

### Requirement 7: CI/CD Pipeline Implementation

**User Story:** As a maintainer, I want automated build and publish processes, so that I can release packages consistently without manual intervention.

#### Acceptance Criteria

1. WHEN setting up CI/CD, THE System SHALL create GitHub Actions workflows for automated building
2. WHEN triggering builds, THE System SHALL run workflows on push to main branch and pull requests
3. WHEN publishing packages, THE System SHALL automate NuGet package creation and publishing to GitHub Packages
4. WHEN running CI, THE System SHALL execute all tests and quality checks before publishing
5. WHEN versioning releases, THE System SHALL support both manual and automated version bumping

### Requirement 8: Quality and Debugging Support

**User Story:** As a developer debugging issues, I want access to source code and symbols, so that I can effectively troubleshoot problems in my applications.

#### Acceptance Criteria

1. WHEN building packages, THE System SHALL enable deterministic builds for reproducible outputs
2. WHEN creating packages, THE System SHALL generate symbols packages (.snupkg) alongside main packages
3. WHEN packaging source code, THE System SHALL include source code in symbols packages for source debugging
4. WHEN configuring builds, THE System SHALL enable SourceLink for source code navigation in debuggers
5. WHEN validating packages, THE System SHALL run package validation tools to ensure quality standards

### Requirement 9: Documentation and Usage Examples

**User Story:** As a new user of the packages, I want comprehensive documentation with examples, so that I can quickly understand how to integrate and use the packages.

#### Acceptance Criteria

1. WHEN creating documentation, THE System SHALL provide a comprehensive README.md file in the repository root
2. WHEN documenting usage, THE System SHALL include code examples for each package's primary functionality
3. WHEN explaining installation, THE System SHALL provide clear NuGet package installation instructions
4. WHEN documenting configuration, THE System SHALL explain how to configure and use GitHub Packages as the package source
5. WHEN providing examples, THE System SHALL include sample projects demonstrating package integration

### Requirement 10: Project Structure and Organization

**User Story:** As a contributor, I want a well-organized project structure following .NET best practices, so that I can easily navigate and contribute to the codebase.

#### Acceptance Criteria

1. WHEN organizing the repository, THE System SHALL follow standard .NET solution structure with `src/` directory for source projects
2. WHEN creating build configuration, THE System SHALL place `Directory.Build.props` and `Directory.Packages.props` at the repository root
3. WHEN organizing workflows, THE System SHALL place GitHub Actions workflows in `.github/workflows/` directory
4. WHEN structuring documentation, THE System SHALL include appropriate documentation files at the repository root
5. WHEN configuring IDE support, THE System SHALL include `.editorconfig` for consistent code formatting