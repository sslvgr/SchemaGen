# Package Publishing Guide

This document describes the automated package publishing workflow for the SchemaGen project.

## Overview

The SchemaGen project uses GitHub Actions to automatically build, validate, and publish NuGet packages to both GitHub Packages and NuGet.org. The workflow includes comprehensive error handling, retry logic, and validation to ensure reliable package publishing.

## Publishing Workflow Features

### 1. Automated Package Validation
- **Package Validation Tool**: Uses Microsoft.DotNet.PackageValidation.Tool to validate all packages before publishing
- **Comprehensive Validation**: Checks package structure, metadata, and compliance with NuGet standards
- **Fail-Fast Approach**: Stops the publishing process if any package fails validation

### 2. Error Handling and Retry Logic
- **Exponential Backoff**: Implements retry logic with exponential backoff for transient failures
- **Maximum Retry Attempts**: Up to 3 retry attempts for each package publishing operation
- **Timeout Protection**: 300-second timeout for each publishing attempt to prevent hanging
- **Detailed Error Reporting**: Clear error messages and status indicators for troubleshooting

### 3. Authentication Verification
- **GitHub Packages**: Verifies GITHUB_TOKEN is available before attempting to publish
- **NuGet.org**: Verifies NUGET_API_KEY is available before attempting to publish
- **Early Failure Detection**: Fails fast if authentication credentials are missing

### 4. Package Verification
- **Pre-Publish Verification**: Confirms packages were created successfully before attempting to publish
- **Symbol Package Detection**: Warns if symbol packages are not found but continues with main packages
- **Artifact Verification**: Verifies downloaded artifacts exist before publishing to NuGet.org

## Workflow Triggers

### Automatic Triggers
- **GitHub Releases**: Automatically triggered when a new release is published
- **Release Types**: Supports both regular releases and prereleases

### Manual Triggers
- **Workflow Dispatch**: Can be manually triggered with custom version and prerelease settings
- **Version Override**: Allows specifying a custom version for manual publishing
- **Prerelease Flag**: Option to mark packages as prerelease during manual publishing

## Publishing Destinations

### GitHub Packages (Primary)
- **Registry**: `https://nuget.pkg.github.com/sslvgr/index.json`
- **Authentication**: Uses GITHUB_TOKEN (automatically provided by GitHub Actions)
- **Package Types**: Both main packages (.nupkg) and symbol packages (.snupkg)
- **Visibility**: Private packages accessible to organization members

### NuGet.org (Secondary)
- **Registry**: `https://api.nuget.org/v3/index.json`
- **Authentication**: Uses NUGET_API_KEY secret (must be configured manually)
- **Package Types**: Both main packages (.nupkg) and symbol packages (.snupkg)
- **Visibility**: Public packages accessible to all developers
- **Condition**: Only publishes for non-prerelease versions

## Package Types Generated

### Main Packages (.nupkg)
- **SchemaGen.Core**: Meta-package referencing all core packages
- **SchemaGen.Core.Markdown**: Markdown schema generation functionality
- **SchemaGen.Core.Mermaid**: Mermaid diagram generation functionality
- **SchemaGen.Core.SqlDdl**: SQL DDL generation functionality
- **SchemaGen.Tool**: Command-line tool for schema generation

### Symbol Packages (.snupkg)
- **Debugging Support**: Contains debugging symbols and source code
- **SourceLink Integration**: Enables source code navigation in debuggers
- **Deterministic Builds**: Ensures reproducible debugging experience

## Workflow Steps

### Build Phase
1. **Checkout**: Retrieves source code with full Git history for versioning
2. **Setup .NET**: Configures .NET SDK environment
3. **Cache Dependencies**: Caches NuGet packages for faster builds
4. **Restore**: Downloads and restores all package dependencies
5. **Build**: Compiles all projects in Release configuration
6. **Test**: Runs all unit tests to ensure code quality
7. **Package**: Creates NuGet packages with proper versioning

### Validation Phase
1. **Package Verification**: Confirms packages were created successfully
2. **Tool Installation**: Installs Microsoft.DotNet.PackageValidation.Tool
3. **Package Validation**: Validates all packages against NuGet standards
4. **Authentication Check**: Verifies publishing credentials are available

### Publishing Phase
1. **GitHub Packages**: Publishes main and symbol packages to GitHub Packages
2. **Artifact Upload**: Uploads packages as GitHub Actions artifacts
3. **Release Attachment**: Attaches packages to GitHub release (if applicable)
4. **NuGet.org Publishing**: Publishes to NuGet.org for non-prerelease versions

## Error Scenarios and Handling

### Build Failures
- **Restore Failures**: Clear error messages for dependency resolution issues
- **Compilation Errors**: Detailed build output for troubleshooting
- **Test Failures**: Stops publishing if any tests fail

### Validation Failures
- **Package Structure**: Reports specific validation errors for each package
- **Metadata Issues**: Identifies missing or incorrect package metadata
- **Compliance Problems**: Highlights NuGet standard compliance issues

### Publishing Failures
- **Network Issues**: Automatic retry with exponential backoff
- **Authentication Problems**: Clear error messages for credential issues
- **Duplicate Packages**: Uses --skip-duplicate flag to handle version conflicts
- **Timeout Issues**: 300-second timeout prevents hanging operations

## Configuration Requirements

### GitHub Secrets
- **GITHUB_TOKEN**: Automatically provided by GitHub Actions (no configuration needed)
- **NUGET_API_KEY**: Must be manually configured in repository secrets for NuGet.org publishing

### Environment Protection
- **Production Environment**: GitHub Packages publishing uses production environment protection
- **NuGet.org Environment**: Separate environment protection for NuGet.org publishing
- **Manual Approval**: Can be configured to require manual approval for sensitive operations

## Monitoring and Troubleshooting

### Workflow Logs
- **Detailed Logging**: Comprehensive logs for each step of the publishing process
- **Status Indicators**: Clear success (✅) and failure (❌) indicators
- **Progress Tracking**: Step-by-step progress reporting throughout the workflow

### Artifact Management
- **Retention Policy**: Artifacts retained for 90 days
- **Version Tagging**: Artifacts tagged with release version for easy identification
- **Download Access**: Artifacts available for download from GitHub Actions interface

### Common Issues
1. **Missing Secrets**: Ensure NUGET_API_KEY is configured in repository secrets
2. **Validation Failures**: Check package metadata and structure requirements
3. **Network Timeouts**: Retry logic handles most transient network issues
4. **Version Conflicts**: Use --skip-duplicate flag to handle existing versions

## Best Practices

### Version Management
- **Semantic Versioning**: Follow SemVer for all package versions
- **Git-Based Versioning**: Use MinVer for automatic version calculation
- **Consistent Versioning**: All packages use the same version number

### Quality Assurance
- **Automated Testing**: Ensure all tests pass before publishing
- **Package Validation**: Always validate packages before publishing
- **Symbol Packages**: Include debugging symbols for better developer experience

### Security
- **Secret Management**: Use GitHub Secrets for sensitive credentials
- **Environment Protection**: Use environment protection rules for production publishing
- **Audit Trail**: Maintain clear audit trail through GitHub Actions logs

## Future Enhancements

### Planned Improvements
- **Package Source Mapping**: Configure package source mapping to eliminate warnings
- **Advanced Validation**: Additional custom validation rules for package quality
- **Notification Integration**: Slack or email notifications for publishing events
- **Rollback Capability**: Automated rollback for failed deployments

### Monitoring Enhancements
- **Health Checks**: Automated health checks after package publishing
- **Usage Analytics**: Track package download and usage statistics
- **Performance Monitoring**: Monitor publishing performance and optimization opportunities
