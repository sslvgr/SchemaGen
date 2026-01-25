using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SchemaGen.Core.SqlDdl.SchemaGen;

/// <summary>
/// Generates SQL DDL (Data Definition Language) scripts from Entity Framework DbContext.
/// </summary>
public static class SqlDdlGenerator
{
    /// <summary>
    /// Generates a SQL DDL script for the specified DbContext.
    /// </summary>
    /// <param name="context">The Entity Framework DbContext to generate DDL for.</param>
    /// <returns>A string containing the SQL DDL script with CREATE statements for all tables, indexes, and constraints.</returns>
    public static string Generate(DbContext context)
    {
        var sb = new StringBuilder();

        sb.AppendLine("-- =====================================================");
        sb.AppendLine("-- Database Schema DDL");
        sb.AppendLine($"-- DbContext: {context.GetType().Name}");
        sb.AppendLine($"-- Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine("-- =====================================================");
        sb.AppendLine();

        try
        {
            var script = context.Database.GenerateCreateScript();
            sb.AppendLine(script);
        }
        catch (Exception ex)
        {
            sb.AppendLine("-- ERROR: Failed to generate DDL script");
            sb.AppendLine($"-- {ex.Message}");
            sb.AppendLine();
            sb.AppendLine("-- Note: DDL generation requires a valid database connection.");
            sb.AppendLine("-- Make sure connection strings are configured correctly.");
        }

        return sb.ToString();
    }
}
