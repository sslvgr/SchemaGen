using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SchemaGen;

Console.WriteLine("==============================================");
Console.WriteLine("   SchemaGen (dotnet tool)");
Console.WriteLine("==============================================");
Console.WriteLine();

var parseResult = ArgumentParser.Parse(args);

if (!parseResult.Success)
{
    Console.Error.WriteLine(parseResult.ErrorMessage);
    Console.WriteLine();
    Console.WriteLine(ArgumentParser.GetUsage());
    return 1;
}

var options = parseResult.Options!;

try
{
    var assemblyPath = options.AssemblyPath;
    if (string.IsNullOrWhiteSpace(assemblyPath))
    {
        assemblyPath = BuildAndResolveAssemblyPath(options);
    }

    if (string.IsNullOrWhiteSpace(assemblyPath) || !File.Exists(assemblyPath))
    {
        Console.Error.WriteLine($"ERROR: Assembly not found: {assemblyPath}");
        return 1;
    }

    assemblyPath = Path.GetFullPath(assemblyPath);

    using var loader = new TargetAssemblyLoader(assemblyPath);
    var assemblies = loader.LoadAllAssembliesInDirectory();
    var factories = assemblies
        .SelectMany(DesignTimeDbContextFactoryDiscovery.DiscoverFactories)
        .GroupBy(f => (f.FactoryType, f.ContextType))
        .Select(g => g.First())
        .ToList();

    if (options.DebugDiscovery)
    {
        DebugDiscoveryPrinter.Print(assemblies, factories);
        Console.WriteLine();
    }

    if (options.ListContexts)
    {
        if (factories.Count == 0)
        {
            Console.WriteLine("No design-time DbContext factories found.");
            Console.WriteLine("Expected: classes implementing IDesignTimeDbContextFactory<TContext>.");
            return 0;
        }

        Console.WriteLine("Available contexts (via design-time factories):");
        foreach (var factory in factories.OrderBy(f => f.ContextType.FullName, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"- {factory.ContextType.FullName}");
        }

        return 0;
    }

    if (factories.Count == 0)
    {
        Console.Error.WriteLine("ERROR: No design-time DbContext factories found in the target assembly.");
        Console.Error.WriteLine(
            "Add classes implementing IDesignTimeDbContextFactory<TContext> (same pattern as EF migrations).");
        return 1;
    }

    var selectedFactories = FactorySelector.SelectFactories(factories, options.Context);
    if (selectedFactories.Count == 0)
    {
        Console.Error.WriteLine($"ERROR: Context '{options.Context}' not found.");
        Console.WriteLine();
        Console.WriteLine("Use --list-contexts to see available contexts.");
        return 1;
    }

    Console.WriteLine($"Output: {options.OutputDirectory}");
    Console.WriteLine();

    foreach (var factory in selectedFactories)
    {
        using var context = factory.CreateDbContext(options.FactoryArgs);
        DatabaseSchemaGenerator.WriteAll(context, options.OutputDirectory, Console.Out);
    }

    Console.WriteLine("==============================================");
    Console.WriteLine("✓ Schema generation completed successfully!");
    Console.WriteLine("==============================================");
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine("==============================================");
    Console.WriteLine("✗ ERROR: Schema generation failed");
    Console.WriteLine("==============================================");
    Console.WriteLine();
    Console.WriteLine(ex.ToString());
    return 1;
}

static string? BuildAndResolveAssemblyPath(SchemaGenOptions options)
{
    if (string.IsNullOrWhiteSpace(options.ProjectPath))
    {
        return null;
    }

    var projectPath = Path.GetFullPath(options.ProjectPath);
    if (!File.Exists(projectPath))
    {
        Console.Error.WriteLine($"ERROR: Project not found: {projectPath}");
        return null;
    }

    var configuration = options.Configuration ?? "Debug";
    var tfm = options.TargetFramework ?? "net8.0";

    Console.WriteLine($"Building: {projectPath} ({configuration} | {tfm})");
    Console.WriteLine();

    var dotnetArgs = $"build \"{projectPath}\" -c {configuration}";

    var process = Process.Start(
        new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = dotnetArgs,
            UseShellExecute = false
        });

    process?.WaitForExit();

    if (process == null || process.ExitCode != 0)
    {
        Console.Error.WriteLine("ERROR: dotnet build failed.");
        return null;
    }

    var assemblyName = CsprojMetadataReader.TryGetAssemblyName(projectPath)
                       ?? Path.GetFileNameWithoutExtension(projectPath);

    var assemblyPath = Path.Combine(
        Path.GetDirectoryName(projectPath)!,
        "bin",
        configuration,
        tfm,
        $"{assemblyName}.dll"
    );

    return File.Exists(assemblyPath) ? assemblyPath : null;
}

file sealed class SchemaGenOptions
{
    public string? AssemblyPath { get; init; }
    public string? ProjectPath { get; init; }
    public string OutputDirectory { get; init; } = "docs/database";
    public string Context { get; init; } = "all";
    public bool ListContexts { get; init; }
    public bool DebugDiscovery { get; init; }
    public string[] FactoryArgs { get; init; } = [];
    public string? Configuration { get; init; }
    public string? TargetFramework { get; init; }
}

file static class ArgumentParser
{
    public static string GetUsage()
    {
        return """
               Usage:
                 schemagen --assembly <path-to-dll> [--context <name|all>] [--output <dir>] [--list-contexts] [-- --factory-args...]
                 schemagen --project <path-to-csproj> [--configuration Debug|Release] [--tfm net8.0] [--context <name|all>] [--output <dir>] [--list-contexts] [-- --factory-args...]

               Notes:
                 - The target assembly must contain design-time factories (IDesignTimeDbContextFactory<TContext>).
                 - Use '--' to pass additional args to the factory CreateDbContext(args).
                 - Use --debug to print discovery diagnostics.
               """;
    }

    public static (bool Success, string? ErrorMessage, SchemaGenOptions? Options) Parse(string[] args)
    {
        var toolArgs = args;
        var factoryArgs = Array.Empty<string>();

        var passthroughIndex = Array.IndexOf(args, value: "--");
        if (passthroughIndex >= 0)
        {
            toolArgs = args.Take(passthroughIndex).ToArray();
            factoryArgs = args.Skip(passthroughIndex + 1).ToArray();
        }

        var assemblyPath = ReadValue("--assembly");
        var projectPath = ReadValue("--project");
        var context = ReadValue("--context") ?? "all";
        var output = ReadValue("--output") ?? "docs/database";
        var list = HasFlag("--list-contexts");
        var debug = HasFlag("--debug");
        var configuration = ReadValue("--configuration");
        var tfm = ReadValue("--tfm");

        if (string.IsNullOrWhiteSpace(assemblyPath) && string.IsNullOrWhiteSpace(projectPath))
        {
            return (false, "ERROR: Specify --assembly <path> or --project <csproj>.", null);
        }

        return (true, null, new SchemaGenOptions
        {
            AssemblyPath = assemblyPath,
            ProjectPath = projectPath,
            OutputDirectory = output,
            Context = context,
            ListContexts = list,
            DebugDiscovery = debug,
            FactoryArgs = factoryArgs,
            Configuration = configuration,
            TargetFramework = tfm
        });

        bool HasFlag(string flag)
        {
            return toolArgs.Any(a => a.Equals(flag, StringComparison.OrdinalIgnoreCase));
        }

        string? ReadValue(string key)
        {
            for (var i = 0; i < toolArgs.Length; i++)
            {
                var arg = toolArgs[i];
                if (arg.Equals(key, StringComparison.OrdinalIgnoreCase) && i + 1 < toolArgs.Length)
                {
                    return toolArgs[i + 1];
                }

                if (arg.StartsWith(key + "=", StringComparison.OrdinalIgnoreCase))
                {
                    return arg[(key.Length + 1)..];
                }
            }

            return null;
        }
    }
}

file static class CsprojMetadataReader
{
    public static string? TryGetAssemblyName(string projectPath)
    {
        try
        {
            var content = File.ReadAllText(projectPath);
            var match = Regex.Match(
                content,
                pattern: "<AssemblyName>(?<name>[^<]+)</AssemblyName>",
                RegexOptions.IgnoreCase);
            return match.Success ? match.Groups["name"].Value.Trim() : null;
        }
        catch
        {
            return null;
        }
    }
}

file sealed class TargetAssemblyLoader : IDisposable
{
    private readonly AssemblyDependencyResolver _resolver;
    private readonly string _mainAssemblyPath;
    private readonly string _assemblyDirectory;

    public TargetAssemblyLoader(string mainAssemblyPath)
    {
        _mainAssemblyPath = mainAssemblyPath;
        _assemblyDirectory = Path.GetDirectoryName(mainAssemblyPath)!;
        _resolver = new AssemblyDependencyResolver(mainAssemblyPath);

        AssemblyLoadContext.Default.Resolving += Resolve;
    }

    public List<Assembly> LoadAllAssembliesInDirectory()
    {
        var loaded = new List<Assembly?>();

        var main = TryLoadFromPath(_mainAssemblyPath);
        if (main != null)
        {
            loaded.Add(main);
        }

        foreach (var dll in Directory.EnumerateFiles(_assemblyDirectory, searchPattern: "*.dll"))
        {
            var asm = TryLoadFromPath(dll);
            if (asm != null)
            {
                loaded.Add(asm);
            }
        }

        return loaded
            .Where(a => a != null)
            .Cast<Assembly>()
            .DistinctBy(a => a.FullName ?? a.GetName().FullName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private Assembly? Resolve(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        var resolvedPath = _resolver.ResolveAssemblyToPath(assemblyName);
        if (resolvedPath != null)
        {
            return TryLoadFromPath(resolvedPath);
        }

        var localCandidate = Path.Combine(_assemblyDirectory, $"{assemblyName.Name}.dll");
        return File.Exists(localCandidate) ? TryLoadFromPath(localCandidate) : null;
    }

    private static Assembly? TryLoadFromPath(string path)
    {
        try
        {
            return AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(path));
        }
        catch (FileLoadException)
        {
            var fullPath = Path.GetFullPath(path);
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(
                    a =>
                        !string.IsNullOrWhiteSpace(a.Location) &&
                        string.Equals(a.Location, fullPath, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return null;
        }
    }

    public void Dispose()
    {
        AssemblyLoadContext.Default.Resolving -= Resolve;
    }
}

file static class DesignTimeDbContextFactoryDiscovery
{
    private const string DESIGN_TIME_FACTORY_INTERFACE_NAME =
        "Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory`1";

    public static IEnumerable<DiscoveredFactory> DiscoverFactories(Assembly assembly)
    {
        IEnumerable<Type> types;
        try
        {
            types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            types = ex.Types.Where(t => t != null)!;
        }
        catch
        {
            yield break;
        }

        foreach (var type in types)
        {
            if (type.IsAbstract || type.IsInterface)
            {
                continue;
            }

            Type? contextType = null;

            try
            {
                var factoryInterface = type.GetInterfaces()
                    .FirstOrDefault(
                        i =>
                            i.IsGenericType &&
                            string.Equals(
                                i.GetGenericTypeDefinition().FullName,
                                DESIGN_TIME_FACTORY_INTERFACE_NAME,
                                StringComparison.Ordinal)
                    );

                contextType = factoryInterface?.GetGenericArguments()[0];
            }
            catch
            {
                contextType = null;
            }

            if (contextType == null)
            {
                var createMethod = type.GetMethod(
                    name: "CreateDbContext",
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    [typeof(string[])],
                    null
                );

                contextType = createMethod?.ReturnType;
            }

            if (contextType == null)
            {
                continue;
            }

            if (!typeof(DbContext).IsAssignableFrom(contextType))
            {
                continue;
            }

            yield return new DiscoveredFactory(type, contextType);
        }
    }
}

file sealed class DiscoveredFactory(Type factoryType, Type contextType)
{
    public Type FactoryType { get; } = factoryType;
    public Type ContextType { get; } = contextType;

    public DbContext CreateDbContext(string[] args)
    {
        var instance = Activator.CreateInstance(FactoryType)
                       ?? throw new InvalidOperationException(
                           $"Failed to create factory instance: {FactoryType.FullName}");

        var method = FactoryType.GetMethod(
            name: "CreateDbContext",
            BindingFlags.Public | BindingFlags.Instance,
            null,
            [typeof(string[])],
            null
        );
        if (method == null)
        {
            throw new InvalidOperationException(
                $"Factory does not have CreateDbContext(string[]) method: {FactoryType.FullName}");
        }

        var result = method.Invoke(instance, [args]);
        if (result is not DbContext dbContext)
        {
            throw new InvalidOperationException(
                $"CreateDbContext did not return DbContext for factory: {FactoryType.FullName}");
        }

        return dbContext;
    }
}

file static class FactorySelector
{
    public static List<DiscoveredFactory> SelectFactories(List<DiscoveredFactory> factories, string contextArg)
    {
        if (string.Equals(contextArg, b: "all", StringComparison.OrdinalIgnoreCase))
        {
            return factories;
        }

        return factories
            .Where(
                f =>
                    string.Equals(f.ContextType.Name, contextArg, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(f.ContextType.FullName, contextArg, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(
                        f.ContextType.Name.Replace(
                            oldValue: "DbContext",
                            newValue: "",
                            StringComparison.OrdinalIgnoreCase),
                        contextArg,
                        StringComparison.OrdinalIgnoreCase)
            )
            .ToList();
    }
}

file static class DebugDiscoveryPrinter
{
    public static void Print(List<Assembly> assemblies, List<DiscoveredFactory> factories)
    {
        Console.WriteLine($"Debug: loaded assemblies = {assemblies.Count}");
        Console.WriteLine($"Debug: discovered factories = {factories.Count}");

        Console.WriteLine("Debug: non-framework assemblies (first 50)");
        foreach (var name in assemblies
                     .Select(a => a.GetName().Name)
                     .Where(n => !string.IsNullOrWhiteSpace(n))
                     .Cast<string>()
                     .Where(
                         n =>
                             !n.StartsWith(value: "Microsoft.", StringComparison.OrdinalIgnoreCase) &&
                             !n.StartsWith(value: "System.", StringComparison.OrdinalIgnoreCase) &&
                             !string.Equals(n, b: "System", StringComparison.OrdinalIgnoreCase) &&
                             !string.Equals(n, b: "netstandard", StringComparison.OrdinalIgnoreCase))
                     .Distinct(StringComparer.OrdinalIgnoreCase)
                     .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                     .Take(50))
        {
            Console.WriteLine($"  - {name}");
        }

        var factoryNames = factories
            .Select(f => $"{f.FactoryType.FullName} -> {f.ContextType.FullName}")
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (factoryNames.Count != 0)
        {
            Console.WriteLine("Debug: factories");
            foreach (var item in factoryNames)
            {
                Console.WriteLine($"  - {item}");
            }
        }

        Console.WriteLine("Debug: assemblies with CreateDbContext(string[])");
        foreach (var assembly in assemblies.OrderBy(a => a.GetName().Name, StringComparer.OrdinalIgnoreCase))
        {
            var (types, loadErrors) = TryGetLoadableTypes(assembly);
            if (loadErrors.Count != 0)
            {
                Console.WriteLine($"- {assembly.GetName().Name} (type load errors: {loadErrors.Count})");
                foreach (var err in loadErrors.Take(5))
                {
                    Console.WriteLine($"  ! {err}");
                }
                continue;
            }

            var candidates = types
                .Where(
                    t => t.GetMethod(
                        name: "CreateDbContext",
                        BindingFlags.Public | BindingFlags.Instance,
                        null,
                        [typeof(string[])],
                        null
                    ) != null)
                .Select(t => t.FullName)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Take(10)
                .ToList();

            if (candidates.Count == 0)
            {
                continue;
            }

            Console.WriteLine($"- {assembly.GetName().Name}");
            foreach (var candidate in candidates)
            {
                Console.WriteLine($"  - {candidate}");
            }
        }
    }

    private static (IEnumerable<Type> Types, List<string> LoadErrors) TryGetLoadableTypes(Assembly assembly)
    {
        try
        {
            return (assembly.GetTypes(), []);
        }
        catch (ReflectionTypeLoadException ex)
        {
            var types = ex.Types.Where(t => t != null).Cast<Type>();
            var errors = ex.LoaderExceptions
                .Where(e => e != null)
                .Select(e => e!.Message)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            return (types, errors);
        }
        catch
        {
            return ([], ["Unknown type load error."]);
        }
    }
}
