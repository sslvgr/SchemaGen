using Microsoft.EntityFrameworkCore;

namespace SchemaGen.Core.Mermaid.SchemaGen;

/// <summary>
/// Provides Mermaid diagram generation functionality for database schemas.
/// </summary>
public static class MermaidSchemaGenerator
{
    /// <summary>
    /// The default filename for Mermaid diagram files.
    /// </summary>
    public const string MERMAID_DIAGRAM_FILE_NAME = "diagram.md";

    /// <summary>
    /// Generates a Mermaid ERD diagram from the specified DbContext.
    /// </summary>
    /// <param name="context">The Entity Framework DbContext to generate the diagram from.</param>
    /// <returns>A string containing the Mermaid ERD diagram in Markdown format.</returns>
    public static string Generate(DbContext context)
    {
        return MermaidErdGenerator.Generate(context);
    }

    /// <summary>
    /// Generates and writes a Mermaid ERD diagram to a file.
    /// </summary>
    /// <param name="context">The Entity Framework DbContext to generate the diagram from.</param>
    /// <param name="outputDirectory">The directory where the diagram file should be written.</param>
    /// <param name="log">Optional TextWriter for logging output.</param>
    /// <returns>The path to the generated diagram file.</returns>
    public static string WriteToFile(DbContext context, string outputDirectory, TextWriter? log = null)
    {
        var contextName = GetDefaultContextFolderName(context);
        var contextDir = Path.Combine(outputDirectory, contextName);

        Directory.CreateDirectory(contextDir);

        log?.WriteLine($"Generating Mermaid ERD for {context.GetType().Name}...");

        var mermaidPath = Path.Combine(contextDir, MERMAID_DIAGRAM_FILE_NAME);
        File.WriteAllText(mermaidPath, Generate(context));
        log?.WriteLine($"  ✓ Mermaid ERD: {mermaidPath}");

        return mermaidPath;
    }

    private static string GetDefaultContextFolderName(DbContext context)
    {
        return context.GetType().Name.Replace(oldValue: "DbContext", newValue: "", StringComparison.Ordinal)
            .ToLowerInvariant();
    }
}
