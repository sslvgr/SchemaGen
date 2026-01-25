using Microsoft.EntityFrameworkCore;
using SchemaGen.Core.Markdown.SchemaGen;
using SchemaGen.Core.Mermaid.SchemaGen;
using SchemaGen.Core.SqlDdl.SchemaGen;

namespace SchemaGen;

/// <summary>
/// Unified database schema generator that combines Markdown, Mermaid, and SQL DDL generation.
/// </summary>
public static class DatabaseSchemaGenerator
{
    /// <summary>
    /// The default filename for Markdown documentation files.
    /// </summary>
    public const string MARKDOWN_FILE_NAME = "README.md";

    /// <summary>
    /// Generates and writes all schema documentation formats (Markdown, Mermaid, SQL DDL) for the given DbContext.
    /// </summary>
    /// <param name="context">The Entity Framework DbContext to generate documentation for.</param>
    /// <param name="outputDirectory">The directory where all documentation files will be written.</param>
    /// <param name="log">Optional TextWriter for logging output.</param>
    public static void WriteAll(DbContext context, string outputDirectory, TextWriter? log = null)
    {
        var contextName = GetDefaultContextFolderName(context);
        var contextDir = Path.Combine(outputDirectory, contextName);

        Directory.CreateDirectory(contextDir);

        log?.WriteLine($"Generating schema documentation for {context.GetType().Name}...");

        // Generate Markdown documentation
        var markdownPath = Path.Combine(contextDir, MARKDOWN_FILE_NAME);
        File.WriteAllText(markdownPath, MarkdownSchemaGenerator.Generate(context));
        log?.WriteLine($"  ✓ Markdown: {markdownPath}");

        // Generate Mermaid ERD diagram
        MermaidSchemaGenerator.WriteToFile(context, outputDirectory, log);

        // Generate SQL DDL script
        SqlDdlSchemaGenerator.WriteToFile(context, outputDirectory, log);

        log?.WriteLine($"Schema documentation generated successfully in: {contextDir}");
        log?.WriteLine();
    }

    private static string GetDefaultContextFolderName(DbContext context)
    {
        return context.GetType().Name.Replace(oldValue: "DbContext", newValue: "", StringComparison.Ordinal)
            .ToLowerInvariant();
    }
}
