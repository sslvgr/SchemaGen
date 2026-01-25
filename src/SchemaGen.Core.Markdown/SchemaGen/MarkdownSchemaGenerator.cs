using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SchemaGen.Core.Markdown.SchemaGen;

/// <summary>
/// Generates Markdown documentation for database schemas from Entity Framework DbContext.
/// </summary>
public static class MarkdownSchemaGenerator
{
    /// <summary>
    /// Generates a comprehensive Markdown documentation for the database schema.
    /// </summary>
    /// <param name="context">The Entity Framework DbContext to generate documentation for.</param>
    /// <returns>A string containing the complete Markdown documentation of the database schema.</returns>
    public static string Generate(DbContext context)
    {
        var sb = new StringBuilder();
        var model = context.Model;

        sb.AppendLine("# Database Schema");
        sb.AppendLine();
        sb.AppendLine($"**Generated:** {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine();
        sb.AppendLine($"**DbContext:** `{context.GetType().Name}`");
        sb.AppendLine();

        sb.AppendLine("## Database Information");
        sb.AppendLine();
        sb.AppendLine("- **Schema:** public (PostgreSQL)");
        sb.AppendLine();

        var entityTypes = model.GetEntityTypes()
            .Where(e => e.GetTableName() != null)
            .OrderBy(e => e.GetTableName())
            .ToList();

        sb.AppendLine("## Statistics");
        sb.AppendLine();
        sb.AppendLine($"- **Total Tables:** {entityTypes.Count}");
        sb.AppendLine($"- **Total Foreign Keys:** {entityTypes.Sum(e => e.GetForeignKeys().Count())}");
        sb.AppendLine($"- **Total Indexes:** {entityTypes.Sum(e => e.GetIndexes().Count())}");
        sb.AppendLine();

        sb.AppendLine("## Table of Contents");
        sb.AppendLine();
        foreach (var entityType in entityTypes)
        {
            var tableName = entityType.GetTableName();
            sb.AppendLine(
                $"- [{tableName}](#{tableName?.Replace(oldValue: "_", newValue: "-", StringComparison.Ordinal)})");
        }
        sb.AppendLine();

        sb.AppendLine("## Tables");
        sb.AppendLine();

        foreach (var entityType in entityTypes)
        {
            GenerateTableSection(sb, entityType);
        }

        return sb.ToString();
    }

    private static void GenerateTableSection(StringBuilder sb, IEntityType entityType)
    {
        var tableName = entityType.GetTableName();
        var schema = entityType.GetSchema() ?? "public";

        sb.AppendLine($"### {tableName}");
        sb.AppendLine();
        sb.AppendLine($"**Schema:** `{schema}`");
        sb.AppendLine();
        sb.AppendLine($"**CLR Type:** `{entityType.ClrType.Name}`");
        sb.AppendLine();

        sb.AppendLine("#### Columns");
        sb.AppendLine();
        sb.AppendLine("| Column | Type | Nullable | Default | Description |");
        sb.AppendLine("|--------|------|----------|---------|-------------|");

        var storeObjectId = StoreObjectIdentifier.Table(tableName!, schema);

        foreach (var property in entityType.GetProperties())
        {
            var columnName = property.GetColumnName(storeObjectId) ?? property.Name;
            var columnType = property.GetColumnType();
            var isNullable = property.IsNullable ? "Yes" : "No";
            var defaultValue = property.GetDefaultValueSql() ?? "";

            var description = new List<string>();
            if (property.IsPrimaryKey())
            {
                description.Add("PK");
            }

            if (property.IsForeignKey())
            {
                description.Add("FK");
            }

            if (property.ValueGenerated != ValueGenerated.Never)
            {
                description.Add(property.ValueGenerated.ToString());
            }

            sb.AppendLine(
                $"| `{columnName}` | `{columnType}` | {isNullable} | `{defaultValue}` | {string.Join(separator: ", ", description)} |");
        }
        sb.AppendLine();

        var primaryKey = entityType.FindPrimaryKey();
        if (primaryKey != null)
        {
            sb.AppendLine("#### Primary Key");
            sb.AppendLine();
            var pkColumns = string.Join(
                separator: ", ",
                primaryKey.Properties.Select(p => $"`{p.GetColumnName(storeObjectId) ?? p.Name}`"));
            sb.AppendLine($"- **{primaryKey.GetName()}**: {pkColumns}");
            sb.AppendLine();
        }

        var foreignKeys = entityType.GetForeignKeys().ToList();
        if (foreignKeys.Count != 0)
        {
            sb.AppendLine("#### Foreign Keys");
            sb.AppendLine();
            foreach (var fk in foreignKeys)
            {
                var principalTable = fk.PrincipalEntityType.GetTableName();
                var columns = string.Join(
                    separator: ", ",
                    fk.Properties.Select(p => $"`{p.GetColumnName(storeObjectId) ?? p.Name}`"));
                var principalStoreObjectId = StoreObjectIdentifier.Table(
                    principalTable!,
                    fk.PrincipalEntityType.GetSchema() ?? "public"
                );
                var principalColumns = string.Join(
                    separator: ", ",
                    fk.PrincipalKey.Properties.Select(p => $"`{p.GetColumnName(principalStoreObjectId) ?? p.Name}`"));
                var onDelete = fk.DeleteBehavior;

                sb.AppendLine($"- **{fk.GetConstraintName()}**");
                sb.AppendLine($"  - Columns: {columns}");
                sb.AppendLine($"  - References: `{principalTable}`({principalColumns})");
                sb.AppendLine($"  - On Delete: `{onDelete}`");
            }
            sb.AppendLine();
        }

        var indexes = entityType.GetIndexes().ToList();
        if (indexes.Count != 0)
        {
            sb.AppendLine("#### Indexes");
            sb.AppendLine();
            foreach (var index in indexes)
            {
                var columns = string.Join(
                    separator: ", ",
                    index.Properties.Select(p => $"`{p.GetColumnName(storeObjectId) ?? p.Name}`"));
                var unique = index.IsUnique ? "Unique" : "Non-unique";
                var filter = index.GetFilter();

                sb.AppendLine($"- **{index.GetDatabaseName()}**");
                sb.AppendLine($"  - Columns: {columns}");
                sb.AppendLine($"  - Type: {unique}");
                if (!string.IsNullOrEmpty(filter))
                {
                    sb.AppendLine($"  - Filter: `{filter}`");
                }
            }
            sb.AppendLine();
        }

        var navigations = entityType.GetNavigations().ToList();
        if (navigations.Count != 0)
        {
            sb.AppendLine("#### Relationships");
            sb.AppendLine();
            foreach (var nav in navigations)
            {
                var targetEntity = nav.TargetEntityType.GetTableName();
                var relationType = nav.IsCollection ? "One-to-Many" : "Many-to-One";

                sb.AppendLine($"- **{nav.Name}** → `{targetEntity}` ({relationType})");
            }
            sb.AppendLine();
        }

        sb.AppendLine("---");
        sb.AppendLine();
    }
}
