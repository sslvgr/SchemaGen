using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SchemaGen.Core.Mermaid.SchemaGen;

/// <summary>
/// Generates Mermaid ERD (Entity Relationship Diagram) from Entity Framework DbContext.
/// </summary>
public static class MermaidErdGenerator
{
    /// <summary>
    /// Generates a Mermaid ERD diagram from the specified DbContext.
    /// </summary>
    /// <param name="context">The Entity Framework DbContext to generate the diagram from.</param>
    /// <returns>A string containing the Mermaid ERD diagram in Markdown format.</returns>
    public static string Generate(DbContext context)
    {
        var sb = new StringBuilder();
        var model = context.Model;

        sb.AppendLine("```mermaid");
        sb.AppendLine("erDiagram");
        sb.AppendLine();

        var entityTypes = model.GetEntityTypes()
            .Where(e => e.GetTableName() != null)
            .OrderBy(e => e.GetTableName())
            .ToList();

        foreach (var entityType in entityTypes)
        {
            var tableName = entityType.GetTableName()!;
            var storeObjectId = StoreObjectIdentifier.Table(tableName, entityType.GetSchema() ?? "public");

            sb.AppendLine($"    {tableName} {{");

            foreach (var property in entityType.GetProperties())
            {
                var columnName = property.GetColumnName(storeObjectId) ?? property.Name;
                var columnType = SimplifyType(property.GetColumnType());
                var constraints = GetConstraints(property);

                sb.AppendLine(
                    $"        {columnType} {columnName}{(string.IsNullOrEmpty(constraints) ? "" : $" {constraints}")}");
            }

            sb.AppendLine("    }");
            sb.AppendLine();
        }

        var processedRelationships = new HashSet<string>(StringComparer.Ordinal);

        foreach (var entityType in entityTypes)
        {
            var tableName = entityType.GetTableName()!;

            foreach (var fk in entityType.GetForeignKeys())
            {
                var principalTable = fk.PrincipalEntityType.GetTableName()!;
                var relationshipKey =
                    $"{principalTable}_{tableName}_{string.Join(separator: "_", fk.Properties.Select(p => p.Name))}";

                if (!processedRelationships.Add(relationshipKey))
                {
                    continue;
                }

                var cardinality = GetRelationshipCardinality(fk);
                var label = GetRelationshipLabel(fk);

                sb.AppendLine($"    {principalTable} {cardinality} {tableName} : \"{label}\"");
            }
        }

        sb.AppendLine("```");
        sb.AppendLine();
        sb.AppendLine($"*Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC*");

        return sb.ToString();
    }

    private static string SimplifyType(string columnType)
    {
        if (columnType.StartsWith(value: "character varying", StringComparison.OrdinalIgnoreCase) ||
            columnType.StartsWith(value: "varchar", StringComparison.OrdinalIgnoreCase) ||
            columnType.StartsWith(value: "text", StringComparison.OrdinalIgnoreCase))
        {
            return "string";
        }

        if (columnType.StartsWith(value: "timestamp", StringComparison.OrdinalIgnoreCase) ||
            columnType.StartsWith(value: "date", StringComparison.OrdinalIgnoreCase))
        {
            return "timestamp";
        }

        if (columnType.StartsWith(value: "integer", StringComparison.OrdinalIgnoreCase) ||
            columnType.StartsWith(value: "bigint", StringComparison.OrdinalIgnoreCase) ||
            columnType.StartsWith(value: "smallint", StringComparison.OrdinalIgnoreCase))
        {
            return "int";
        }

        if (columnType.StartsWith(value: "numeric", StringComparison.OrdinalIgnoreCase) ||
            columnType.StartsWith(value: "decimal", StringComparison.OrdinalIgnoreCase))
        {
            return "decimal";
        }

        if (columnType.StartsWith(value: "boolean", StringComparison.OrdinalIgnoreCase))
        {
            return "boolean";
        }

        if (columnType.StartsWith(value: "uuid", StringComparison.OrdinalIgnoreCase))
        {
            return "uuid";
        }

        if (columnType.StartsWith(value: "jsonb", StringComparison.OrdinalIgnoreCase))
        {
            return "jsonb";
        }

        return "string";
    }

    private static string GetConstraints(IProperty property)
    {
        if (property.IsPrimaryKey())
        {
            return "PK";
        }

        if (property.IsForeignKey())
        {
            return "FK";
        }

        return string.Empty;
    }

    private static string GetRelationshipCardinality(IForeignKey fk)
    {
        var isOneToOne = fk.IsUnique;

        if (isOneToOne)
        {
            return fk.IsRequired ? "||--||" : "||--o|";
        }

        return "||--o{";
    }

    private static string GetRelationshipLabel(IForeignKey fk)
    {
        var navigation = fk.DependentToPrincipal?.Name;
        if (!string.IsNullOrEmpty(navigation))
        {
            return navigation;
        }

        return string.Join(separator: ", ", fk.Properties.Select(p => p.Name));
    }
}
