using AutoCEMI.Models;
using AutoCEMI.Services;
using System.CommandLine;
using System.Reflection;
using System.Text.Json;

namespace AutoCEMI.CLI
{
    internal class Program
    {
        static string Greeting = $"AutoCEMI v. {Assembly.GetExecutingAssembly().GetName().Version} [.NET {System.Environment.Version}]";
        static DataService dataService = new DataService();

        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("AutoCEMI Engine - game launcher");

            // --- Options ---
            var noGreetingOption = new Option<bool>("--nogreeting")
            {
                Description = "Suppress the greeting message"
            };

            var statsOption = new Option<bool>("--stats")
            {
                Description = "Display playtime statistics and exit"
            };

            var configOption = new Option<FileInfo?>("--config")
            {
                Description = "Path to the game profile JSON config file. If not specified, the app will run without launching a game."
            };

            var forceOption = new Option<bool>("--force")
            {
                Description = "Force the action without confirmation (for reset commands)"
            };

            rootCommand.Options.Add(noGreetingOption);
            rootCommand.Options.Add(statsOption);
            rootCommand.Options.Add(configOption);
            rootCommand.Options.Add(forceOption);

            // --- Subcommand: add ---
            var addCommand = new Command("add", "Add or update an IWAD, source port, or mod");

            var addTypeArg = new Argument<string>(
                "type");
            addTypeArg.CompletionSources.Add("iwad", "sourceport", "mod");

            var addNameArg = new Argument<string>("name");
            var addPathArg = new Argument<string>("path");

            addCommand.Arguments.Add(addTypeArg);
            addCommand.Arguments.Add(addNameArg);
            addCommand.Arguments.Add(addPathArg);

            addCommand.SetAction(parseResult =>
            {
                var type = parseResult.GetValue(addTypeArg);
                var name = parseResult.GetValue(addNameArg);
                var path = parseResult.GetValue(addPathArg);

                if (name  == null || path == null)
                {
                    Console.WriteLine("Error: name and path arguments are required.");
                    return 0;
                }

                switch (type)
                {
                    case "iwad":
                        dataService.AddIwad(new Iwad(name, path));
                        break;

                    case "sourceport":
                        dataService.AddPort(new SourcePort(name, path));
                        break;

                    case "mod":
                        dataService.AddMod(new Mod(name, path));
                        break;

                    default:
                        Console.WriteLine($"Error: unknown type '{type}'. Must be iwad, sourceport, or mod.");
                        break;
                }

                return 0; // exit code
            });

            // --- Subcommand: remove ---
            var removeCommand = new Command("remove", "Remove an IWAD, source port, or mod");

            var removeTypeArg = new Argument<string>("type");
            removeTypeArg.CompletionSources.Add("iwad", "sourceport", "mod");
            var removeNameArg = new Argument<string>("name");

            removeCommand.Arguments.Add(removeTypeArg);
            removeCommand.Arguments.Add(removeNameArg);

            removeCommand.SetAction(parseResult =>
            {
                var type = parseResult.GetValue(removeTypeArg);
                var name = parseResult.GetValue(removeNameArg);
                if (name == null)
                {
                    Console.WriteLine("Error: name argument is required.");
                    return 0;
                }
                switch (type)
                {
                    case "iwad":
                        dataService.RemoveIwad(name);
                        break;
                    case "sourceport":
                        dataService.RemovePort(name);
                        break;
                    case "mod":
                        dataService.RemoveMod(name);
                        break;
                    default:
                        Console.WriteLine($"Error: unknown type '{type}'. Must be iwad, sourceport, or mod.");
                        break;
                }
                return 0; // exit code
            });

            // ---Subcommand: list ---
            var listCommand = new Command("list", "List all IWADs, source ports, or mods");

            var listTypeArg = new Argument<string>("type");
            listTypeArg.CompletionSources.Add("iwad", "sourceport", "mod");

            listCommand.Arguments.Add(listTypeArg);

            listCommand.SetAction(parseResult =>
            {
                var type = parseResult.GetValue(listTypeArg);
                switch (type)
                {
                    case "iwad":
                        foreach (var iwad in dataService.iwads)
                            Console.WriteLine($"- {iwad.Name}: {iwad.Path}");
                        break;
                    case "sourceport":
                        foreach (var port in dataService.ports)
                            Console.WriteLine($"- {port.Name}: {port.Path}");
                        break;
                    case "mod":
                        foreach (var mod in dataService.mods)
                            Console.WriteLine($"- {mod.Name}: {mod.Path}");
                        break;
                    default:
                        Console.WriteLine($"Error: unknown type '{type}'. Must be iwad, sourceport, or mod.");
                        break;
                }
                return 0; // exit code
            });

            // --- Subcommand: reset-all ---
            var resetAllCommand = new Command("reset-all", "Reset all app data (database + config)");
            resetAllCommand.SetAction(parseResult =>
            {
                var force = parseResult.GetValue(forceOption);
                if (force)
                { dataService.ClearAllData(); return 0; }
                Console.Write("Are you sure you want to reset app data? (y/n): ");
                if (Console.ReadLine()?.Trim().ToLower() == "y")
                    dataService.ClearAllData();
                return 0;
            });

            // --- Subcommand: reset-db ---
            var resetDbCommand = new Command("reset-db", "Reset the database only (not the config file)");
            resetDbCommand.SetAction(parseResult =>
            {
                var force = parseResult.GetValue(forceOption);
                if (force)
                { dataService.ClearData(); return 0; }
                Console.WriteLine("Warning: this will only reset the database, but not the configuration file. " +
                    "If you want to reset the configuration file as well, use \"reset-all\".");
                Console.Write("Are you sure you want to reset the database? (y/n): ");
                if (Console.ReadLine()?.Trim().ToLower() == "y")
                    dataService.ClearData();
                return 0;
            });

            // --- Subcommand: reset-playtime ---
            var resetPlaytimeCommand = new Command("reset-playtime", "Reset playtime statistics");
            resetPlaytimeCommand.SetAction(parseResult =>
            {
                dataService.ResetPlaytime();
                return 0;
            });

            rootCommand.Subcommands.Add(addCommand);
            rootCommand.Subcommands.Add(removeCommand);
            rootCommand.Subcommands.Add(listCommand);
            rootCommand.Subcommands.Add(resetAllCommand);
            rootCommand.Subcommands.Add(resetDbCommand);
            rootCommand.Subcommands.Add(resetPlaytimeCommand);

            // --- Root handler ---
            rootCommand.SetAction(async (parseResult, cancellationToken) =>
            {
                var noGreeting = parseResult.GetValue(noGreetingOption);
                var stats = parseResult.GetValue(statsOption);
                var config = parseResult.GetValue(configOption);

                if (!noGreeting)
                    Console.WriteLine(Greeting);

                if (stats)
                {
                    dataService.DisplayStats();
                    return 0;
                }
                if (config != null)
                {
                    if (!config.Exists)
                    {
                        Console.WriteLine($"Error: config file not found at '{config.FullName}'.");
                        return 0;
                    }

                    string jsonContent = File.ReadAllText(config.FullName);
                    GameProfile? gameProfile = null;
                    try
                    {
                        gameProfile = JsonSerializer.Deserialize<GameProfile>(jsonContent);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Parsing exception: {ex}");
                        return 0;
                    }

                    if (gameProfile == null)
                    {
                        Console.WriteLine("Error: Failed to parse the game profile from the specified config path.");
                        return 0;
                    }
                    else
                    {
                        await new GameExecute().RunAsync(gameProfile);
                    }
                }
                return 0;
            });

            ParseResult parseResult = rootCommand.Parse(args);
            return parseResult.Invoke();
        }
    }
}