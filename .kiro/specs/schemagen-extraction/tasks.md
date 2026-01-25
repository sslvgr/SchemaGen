# Implementation Plan: SchemaGen Extraction and Restructuring

## Overview

This implementation plan converts the SchemaGen extraction and restructuring design into a series of discrete coding tasks. Each task builds incrementally toward a complete standalone NuGet package repository with modern .NET best practices, automated CI/CD, and comprehensive testing.

## Tasks

- [x] 1. Repository Setup and Initial Structure
  - Create new git repository at `C:\src\SchemaGen`
  - Initialize repository with proper .gitignore for .NET projects
  - Create initial directory structure following .NET conventions
  - Set up solution file (.slnx) and basic project structure
  - _Requirements: 1.1, 1.4, 10.1_

- [ ] 2. Central Configuration Setup
  - [x] 2.1 Create Directory.Build.props with centralized properties
    - Configure version management using MinVer
    - Set up multi-targeting for .NET 8, 9, and 10
    - Configure deterministic builds and symbol generation
    - Set common package metadata (authors, license, repository info)
    - _Requirements: 5.1, 5.2, 4.1, 8.1_

  - [x] 2.2 Create Directory.Packages.props for central package management
    - Enable central package management
    - Define Entity Framework Core with floating patch versions
    - Configure global analyzers and build tools
    - _Requirements: 6.2, 6.1_

  - [x] 2.3 Create global.json and .editorconfig files
    - Specify required .NET SDK version
    - Configure consistent code formatting rules
    - _Requirements: 10.5_

- [ ] 3. Extract and Restructure Core Projects
  - [x] 3.1 Extract SchemaGen.Core.Markdown project
    - Copy Markdown-related functionality from original Teez.SchemaGen.Core
    - Create new project file with proper naming and configuration
    - Ensure all Markdown-specific classes are included
    - Update namespaces to match new package structure
    - _Requirements: 1.2, 1.3, 2.1, 2.3_

  - [ ]* 3.2 Write property test for Markdown functionality preservation
    - **Property 1: Functionality Preservation During Extraction**
    - **Validates: Requirements 1.2, 1.3**

  - [x] 3.3 Extract SchemaGen.Core.Mermaid project
    - Copy Mermaid-related functionality from original Teez.SchemaGen.Core
    - Create new project file with proper naming and configuration
    - Ensure all Mermaid-specific classes are included
    - Update namespaces to match new package structure
    - _Requirements: 1.2, 1.3, 2.1, 2.4_

  - [ ]* 3.4 Write property test for Mermaid functionality preservation
    - **Property 1: Functionality Preservation During Extraction**
    - **Validates: Requirements 1.2, 1.3**

  - [x] 3.5 Extract SchemaGen.Core.SqlDdl project
    - Copy SQL DDL generation functionality from original Teez.SchemaGen.Core
    - Create new project file with proper naming and configuration
    - Ensure all SqlDdlGenerator-related classes are included
    - Update namespaces to match new package structure
    - _Requirements: 1.2, 1.3, 2.1, 2.5_

  - [ ]* 3.6 Write property test for SqlDdl functionality preservation
    - **Property 1: Functionality Preservation During Extraction**
    - **Validates: Requirements 1.2, 1.3**

- [ ] 4. Create Meta-Package and Tool Project
  - [x] 4.1 Create SchemaGen.Core meta-package
    - Create project file with no source code
    - Configure package references to Markdown, Mermaid, and SqlDdl packages
    - Set up proper package metadata and description
    - _Requirements: 2.6, 3.1, 3.3_

  - [ ]* 4.2 Write property test for meta-package structure
    - **Property 5: Meta-Package Dependency Structure**
    - **Validates: Requirements 2.6**

  - [x] 4.3 Extract and restructure SchemaGen.Tool project
    - Copy tool functionality from original Teez.SchemaGen.Tool
    - Update project references to use new package structure
    - Configure as .NET tool for global installation
    - Update namespaces and ensure proper CLI functionality
    - _Requirements: 1.2, 1.3, 2.2_

  - [ ]* 4.4 Write unit tests for tool functionality
    - Test CLI argument parsing and command execution
    - Test integration with core packages
    - _Requirements: 1.2, 1.3_

- [ ] 5. Dependency Management and Package Configuration
  - [x] 5.1 Configure package dependencies and versions
    - Ensure all extracted projects reference correct dependencies
    - Validate Entity Framework floating versions
    - Configure inter-package dependencies within solution
    - _Requirements: 1.5, 3.5, 6.1, 6.3_

  - [ ]* 5.2 Write property test for dependency preservation
    - **Property 2: Dependency Preservation**
    - **Validates: Requirements 1.5**

  - [ ]* 5.3 Write property test for dependency version consistency
    - **Property 12: Dependency Version Consistency**
    - **Validates: Requirements 6.1, 6.3, 6.4**

  - [x] 5.4 Configure package metadata for all projects
    - Set MIT license for all packages
    - Configure package descriptions, authors, and URLs
    - Add package icons and repository information
    - _Requirements: 3.1, 3.3, 3.4_

  - [ ]* 5.5 Write property test for package metadata completeness
    - **Property 6: Package Metadata Completeness**
    - **Validates: Requirements 3.1, 3.3, 3.4**

- [ ] 6. Multi-Targeting and Build Configuration
  - [x] 6.1 Configure multi-targeting for all projects
    - Ensure all projects target .NET 8, 9, and 10
    - Validate framework-specific dependencies
    - Test build output for each target framework
    - _Requirements: 4.1, 4.2, 4.3, 4.4_

  - [ ]* 6.2 Write property test for multi-targeting support
    - **Property 8: Multi-Targeting Support**
    - **Validates: Requirements 4.1, 4.2, 4.4**

  - [ ]* 6.3 Write property test for framework compatibility
    - **Property 9: Framework Compatibility**
    - **Validates: Requirements 4.3**

  - [x] 6.4 Configure build quality features
    - Enable deterministic builds across all projects
    - Configure symbol package generation
    - Set up SourceLink for source debugging
    - Enable package validation
    - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5_

  - [ ]* 6.5 Write property test for build quality features
    - **Property 14: Build Quality Features**
    - **Validates: Requirements 8.1, 8.2, 8.3, 8.4**

- [x] 7. Checkpoint - Validate Local Build and Package Generation
  - Ensure all projects build successfully for all target frameworks
  - Verify NuGet packages are generated with correct structure
  - Validate package metadata and dependencies
  - Run all property tests to ensure correctness
  - Ask the user if questions arise

- [ ] 8. GitHub Actions CI/CD Setup
  - [x] 8.1 Create continuous integration workflow
    - Set up build matrix for multiple OS and .NET versions
    - Configure automated testing and package validation
    - Set up artifact publishing for build outputs
    - _Requirements: 7.1, 7.2, 7.4_

  - [ ]* 8.2 Write property test for CI/CD workflow correctness
    - **Property 13: CI/CD Workflow Correctness**
    - **Validates: Requirements 7.2, 7.3, 7.4, 7.5**

  - [x] 8.3 Create package publishing workflow
    - Configure GitHub Packages publishing on releases
    - Set up automated package validation before publishing
    - Configure symbol package publishing
    - Implement proper error handling and retry logic
    - _Requirements: 7.3, 7.5, 3.2_

  - [ ]* 8.4 Write property test for package validation integration
    - **Property 15: Package Validation Integration**
    - **Validates: Requirements 8.5**

- [ ] 9. Version Management and Semantic Versioning
  - [x] 9.1 Configure MinVer for Git-based versioning
    - Set up version calculation from Git tags and commits
    - Configure version prefix and format
    - Test version generation for different Git scenarios
    - _Requirements: 5.2, 5.3, 5.4, 5.5_

  - [ ]* 9.2 Write property test for centralized version management
    - **Property 10: Centralized Version Management**
    - **Validates: Requirements 5.2, 5.3, 5.4**

  - [ ]* 9.3 Write property test for semantic versioning compliance
    - **Property 11: Semantic Versioning Compliance**
    - **Validates: Requirements 5.5**

- [ ] 10. Documentation and Examples
  - [x] 10.1 Create comprehensive README.md
    - Document package overview and architecture
    - Include installation instructions for GitHub Packages
    - Provide usage examples for each package
    - Document build and contribution guidelines
    - _Requirements: 9.1, 9.3, 9.4_

  - [x] 10.2 Create code examples and sample projects
    - Create example projects demonstrating package usage
    - Include examples for Markdown, Mermaid, and SqlDdl functionality
    - Ensure all examples build and run successfully
    - _Requirements: 9.2, 9.5_

  - [ ]* 10.3 Write property test for documentation completeness
    - **Property 16: Documentation Completeness**
    - **Validates: Requirements 9.2**

  - [ ]* 10.4 Write property test for sample project functionality
    - **Property 17: Sample Project Functionality**
    - **Validates: Requirements 9.5**

- [ ] 11. Quality Assurance and Validation
  - [x] 11.1 Run comprehensive package validation
    - Execute NuGet package validation tools
    - Validate package structure and metadata
    - Test package installation and usage scenarios
    - _Requirements: 8.5_

  - [x] 11.2 Perform integration testing
    - Test complete extraction and build process
    - Validate CI/CD pipeline end-to-end
    - Test package publishing and consumption
    - _Requirements: 7.3, 7.4_

  - [ ]* 11.3 Write property tests for package organization
    - **Property 3: Package Naming Consistency**
    - **Property 4: Package Organization Correctness**
    - **Property 7: Inter-Package Dependency Correctness**
    - **Validates: Requirements 2.1, 2.2, 2.3, 2.4, 2.5, 3.5**

- [ ] 12. Final Checkpoint - Complete System Validation
  - Ensure all tests pass including property-based tests
  - Validate complete CI/CD pipeline functionality
  - Verify package publishing to GitHub Packages works correctly
  - Confirm all requirements are met and documented
  - Ask the user if questions arise

## Notes

- Tasks marked with `*` are optional and can be skipped for faster MVP delivery
- Each task references specific requirements for traceability
- Property tests validate universal correctness properties from the design document
- Checkpoints ensure incremental validation and provide opportunities for user feedback
- The implementation follows modern .NET best practices throughout