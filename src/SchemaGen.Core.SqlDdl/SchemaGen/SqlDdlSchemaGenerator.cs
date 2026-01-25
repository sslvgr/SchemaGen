using Microsoft.EntityFrameworkCore;

namespace SchemaGen.Core.SqlDdl.SchemaGen;

/// <summary>
/// Provides high-level functionality for generating and writing SQL DDL scripts from Entity Framework DbContext.
/// </summary>
public static class SqlDdlSchemaGenerator
{
    /// <summary>
    /// The default filename for SQL DDL output files.
    /// </summary>
    public const string SQL_DDL_FILE_NAME = "ddl.sql";

    /// <summary>
    /// Generates SQL DDL script for the given DbContext.
    /// </summary>
    /// <param name="context">The Entity Framework DbContext to generate DDL for.</param>
    /// <returns>SQL DDL script as a string.</returns>
    public static string Generate(DbContext context)
    {
        return SqlDdlGenerator.Generate(context);
    }

    /// <summary>
    /// Writes SQL DDL script to a file in the specified output directory.
    /// </summary>
    /// <param name="context">The Entity Framework DbContext to generate DDL for.</param>
    /// <param name="outputDirectory">The directory where the DDL file will be written.</param>
    /// <param name="log">Optional TextWriter for logging output.</param>
    /// <returns>The path to the generated DDL file.</returns>
    public static string WriteToFile(DbContext context, string outputDirectory, TextWriter? log = null)
    {
        var contextName = GetDefaultContextFolderName(context);
        var contextDir = Path.Combine(outputDirectory, contextName);

        Directory.CreateDirectory(contextDir);

        log?.WriteLine($"Generating SQL DDL for {context.GetType().Name}...");

        var sqlPath = Path.Combine(contextDir, SQL_DDL_FILE_NAME);
        File.WriteAllText(sqlPath, SqlDdlGenerator.Generate(context));
        log?.WriteLine($"  ✓ SQL DDL: {sqlPath}");

        log?.WriteLine($"SQL DDL generated successfully in: {contextDir}");
        log?.WriteLine();

        return sqlPath;
    }

    private static string GetDefaultContextFolderName(DbContext context)
    {
        return context.GetType().Name.Replace(oldValue: "DbContext", newValue: "", StringComparison.Ordinal)
            .ToLowerInvariant();
    }
}
