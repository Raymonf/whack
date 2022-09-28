using WhackTranslationTool;

string? basePath = null; // @"WindowsNoEditor\Mercury\Content\Message";

static string ResolveFileOverwrite(string path)
{
    while (true)
    {
        Console.WriteLine($"A file already exists at '{path}'");
        Console.Write("Overwrite (y/n)? ");

        var input = Console.ReadLine()!.Trim().ToLowerInvariant();

        if (input == "y")
        {
            return path;
        }

        if (input == "n")
        {
            Console.Write("Enter a new path: '");
            return Console.ReadLine()!.Trim();
        }

        Console.WriteLine($"Invalid selection!" + Environment.NewLine);
    }
}

static string GetAssetFilename(string path)
{
    return Path.GetFileNameWithoutExtension(path) + ".uasset";
}

while (true)
{
    Console.WriteLine("[whack Translation Tool]");
    while (basePath == null || !Directory.Exists(basePath))
    {
        if (basePath != null)
            Console.WriteLine("Invalid path to Message directory.");
        Console.Write("Path to Message directory: ");
        basePath = Console.ReadLine()!.Trim();
        continue;
    }
    Console.WriteLine("'e' for export, 'i' for import, 'bulk_import', 'bulk_export', 'q' to quit");
    Console.WriteLine("reminder: don't screw up");
    Console.Write("Action: ");
    var action = Console.ReadLine()!.Trim().ToLowerInvariant();
    switch (action)
    {
        case "bulk_export":
            {
                var files = Directory.GetFiles(basePath, "*.uasset");
                foreach (var tableName in files)
                {
                    var exporter = new TableExporter(Path.Combine(basePath, tableName));

                    var targetPath = Path.GetFileName(Path.ChangeExtension(tableName, "toml"));
                    if (File.Exists(targetPath))
                    {
                        targetPath = ResolveFileOverwrite(targetPath);
                    }

                    Console.WriteLine($"Exporting to {targetPath}");
                    using StreamWriter writer = File.CreateText(targetPath);
                    exporter.WriteTo(writer);
                    writer.Flush();
                }

                break;
            }
        case "bulk_import":
        {
            Console.WriteLine(Environment.NewLine + "[Bulk Import Mode]");
            Console.Write("Enter the path to the directory of TOML files: ");
            var tomlFolder = Console.ReadLine()!;
            Console.Write("Enter the path to the uasset directory: ");
            var assetFolder = Console.ReadLine()!;
            Console.Write("Enter the path to the output uasset directory: ");
            var outputFolder = Console.ReadLine()!;
            var files = Directory.GetFiles(tomlFolder, "*.toml");
            foreach (var tableName in files)
            {
                var assetFilename = GetAssetFilename(tableName);
                var importer = new TableImporter(tableName, Path.Combine(assetFolder, assetFilename));
                var outputPath = Path.Combine(outputFolder, assetFilename);
                Console.WriteLine($"Exporting to {outputPath}");
                importer.Write(outputPath);
            }
            break;
        }
        case "e":
            {
                Console.WriteLine(Environment.NewLine + "[Export Mode]");
                Console.Write("Enter the name of the asset (e.g., 'WelcomeMessage.uasset'): ");
                var tableName = Console.ReadLine()!.Trim();
                var exporter = new TableExporter(Path.Combine(basePath, tableName));

                var targetPath = Path.ChangeExtension(tableName, "toml");
                if (File.Exists(targetPath))
                {
                    targetPath = ResolveFileOverwrite(targetPath);
                }

                using StreamWriter writer = File.CreateText(targetPath);
                exporter.WriteTo(writer);
                writer.Flush();
                break;
            }

        case "i":
            {
                Console.WriteLine(Environment.NewLine + "[Import Mode]");
                Console.Write("Enter the path to the toml file (e.g., 'WelcomeMessage.toml'): ");
                var importPath = Console.ReadLine()!.Trim();
                Console.Write("Enter the path to the asset (or press enter for default): ");
                var assetPath = Console.ReadLine()!.Trim();
                if (assetPath.Length < 1)
                    assetPath = Path.Combine(basePath, GetAssetFilename(importPath));
                var importer = new TableImporter(importPath, assetPath);
                Console.Write("Enter the output path or folder: ");
                var output = Console.ReadLine()!.Trim();
                if (Directory.Exists(output))
                    output = Path.Combine(output, GetAssetFilename(importPath));
                if (File.Exists(output))
                    output = ResolveFileOverwrite(output);
                importer.Write(output);
                break;
            }

        case "q":
            return;

        default:
            Console.WriteLine("Unrecognized option.");
            break;
    }
    Console.WriteLine();
}