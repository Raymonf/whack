using System.Text.Json;
using WTT;

string? basePath = null; // @"WindowsNoEditor\Mercury\Content\Message";
bool? overwrite = null;

static string ResolveFileOverwrite(string path)
{
    while (true)
    {
        var overwrite = YesNoPrompt.Ask(() =>
        {
            Console.WriteLine($"A file already exists at '{path}'");
            Console.Write("Overwrite (Y/n)? ");
        });

        if (overwrite)
        {
            return path;
        }
        else
        {
            Console.Write("Enter a new path: ");
            var newPath = Console.ReadLine()!.Trim();
            if (newPath.Length < 1)
                continue;
            return newPath;
        }
    }
}

static string GetAssetFilename(string path)
{
    return Path.GetFileNameWithoutExtension(path) + ".uasset";
}

const string ExportSettingsFilename = "ExportSettings.json";
const string ImportSettingsFilename = "ImportSettings.json";

while (true)
{
    Console.WriteLine("[whack Translation Tool]");

    // fuck it, hacked in settings!
    var exportSettings = new ExportSettings();
    if (File.Exists(ExportSettingsFilename))
    {
        using var settingsStream = File.OpenRead(ExportSettingsFilename);
        try
        {
            var result = JsonSerializer.Deserialize<ExportSettings>(settingsStream);
            if (result != null)
                exportSettings = result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unable to read export settings:");
            Console.WriteLine(ex);
        }
    }
    else
    {
        try
        {
            File.WriteAllText(ExportSettingsFilename, JsonSerializer.Serialize(exportSettings));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unable to write export settings:");
            Console.WriteLine(ex);
            exportSettings = new();
        }
    }

    var importSettings = new ImportSettings();
    if (File.Exists(ImportSettingsFilename))
    {
        using var settingsStream = File.OpenRead(ImportSettingsFilename);
        try
        {
            var result = JsonSerializer.Deserialize<ImportSettings>(settingsStream);
            if (result != null)
                importSettings = result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unable to read import settings:");
            Console.WriteLine(ex);
            importSettings = new();
        }
    }
    else
    {
        try
        {
            File.WriteAllText(ImportSettingsFilename, JsonSerializer.Serialize(importSettings));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unable to write import settings:");
            Console.WriteLine(ex);
        }
    }

    while (basePath == null || !Directory.Exists(basePath))
    {
        if (basePath != null)
            Console.WriteLine("Invalid path to Message directory.");
        Console.Write("Path to the game's Message directory: ");
        basePath = Console.ReadLine()!.Trim();
        continue;
    }
    Console.WriteLine();
    while (overwrite == null)
    {
        overwrite = YesNoPrompt.Ask(() =>
        {
            Console.WriteLine("! Overwrite mode will read from and overwrite files in your original Message folder.");
            Console.Write("Enable overwrite mode (Y/n)? ");
        });
        Console.WriteLine();
    }
    Console.WriteLine($"Import language: {importSettings.Language}");
    Console.WriteLine("'e' for export, 'i' for import, 'bi' or 'bulk_import', 'be' or 'bulk_export', 'o' to toggle overwrite mode, 'q' to quit");
    Console.WriteLine($"overwrite mode: {(overwrite.Value ? "ON" : "OFF")}");
    Console.WriteLine("reminder: don't screw up");
    Console.Write("Action: ");
    var action = Console.ReadLine()!.Trim().ToLowerInvariant();
    switch (action)
    {
        case "o":
            overwrite = !overwrite;
            Console.WriteLine($"! Overwrite mode is now {(overwrite.Value ? "ON" : "OFF")}");
            break;
        case "be":
        case "bulk_export":
            {
                var files = Directory.GetFiles(basePath, "*.uasset");
                foreach (var tableName in files)
                {
                    var exporter = new TableExporter(Path.Combine(basePath, tableName), exportSettings);

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
        case "bi":
        case "bulk_import":
            {
                Console.WriteLine(Environment.NewLine + "[Bulk Import Mode]");
                Console.Write("Enter the path to the directory of TOML files: ");
                var tomlFolder = Console.ReadLine()!;
                string assetFolder, outputFolder;
                if (!overwrite.Value)
                {
                    Console.Write("Enter the path to the uasset directory: ");
                    assetFolder = Console.ReadLine()!;
                    Console.Write("Enter the path to the output uasset directory: ");
                    outputFolder = Console.ReadLine()!;
                }
                else
                {
                    Console.WriteLine("! Override mode is on: using original Message directory.");
                    assetFolder = basePath;
                    outputFolder = basePath;
                }
                if (assetFolder.Length < 1 || outputFolder.Length < 1)
                {
                    Console.WriteLine($"! Didn't get a value for one of the directories, cancelling.");
                    break;
                }
                var files = Directory.GetFiles(tomlFolder, "*.toml");
                foreach (var tableName in files)
                {
                    var assetFilename = GetAssetFilename(tableName);
                    var importer = new TableImporter(tableName, Path.Combine(assetFolder, assetFilename), importSettings.Language);
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
                if (tableName.Length < 1)
                {
                    Console.WriteLine("! Didn't get a path to an asset, cancelling.");
                    break;
                }

                var exporter = new TableExporter(Path.Combine(basePath, tableName), exportSettings);
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
                var importer = new TableImporter(importPath, assetPath, importSettings.Language);
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