using AcemiEngine.Models;
using AcemiEngine.Services;
using System.Reflection;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AcemiEngine
{
    internal class Program
    {
        static string Usage = @"Usage: 
autocemi [--config <path>] [--add <type: iwad | sourceport | mod> <name> <path>] [--nogreeting] [--stats]
        ";
        static string Greeting = $"AutoCEMI Engine v. {Assembly.GetExecutingAssembly().GetName().Version} .NET {System.Environment.Version}";
        static DataService dataService = new DataService();
        static async Task Main(string[] args)
        {
            bool showGreeting = true;
            bool showStats = false;
            bool showUsage = (args.Count() > 0) ? false : true ;
            bool launchConfigWizard = false;
            string? configPath = null;
            
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--nogreeting":
                        showGreeting = false;
                        break;
                    case "--stats":
                        showStats = true;
                        break;
                    case "--config":
                        if (launchConfigWizard)
                        {
                            Console.WriteLine("Warning: since \"--config-wizard\" flag was passed before, the application will ignore specified game configuration path.");
                        }
                        if (i + 1 < args.Length)
                        {
                            configPath = args[i + 1];
                            i++; // Skip the next argument since it's the config path
                        }
                        else
                        {
                            Console.WriteLine("Error: --config option requires a path argument.");
                            return;
                        }
                        break;
                    case "--config-wizard":
                        if (configPath != null)
                        {
                            Console.WriteLine("Warning: since launch config path was specified before, the application will ignore \"--config-wizard\" flag and start the game instead.");
                            break;
                        }
                        launchConfigWizard = true;
                        break;
                    case "--add":
                        if (i + 3 < args.Length)
                        {
                            var name = args[i + 2];
                            var path = args[i + 3];
                            switch (args[i + 1])
                            {
                                case ("iwad"):
                                    dataService.AddIwad(new Iwad(name,path));
                                    break;
                                case ("sourceport"):
                                    dataService.AddPort(new SourcePort(name,path));
                                    break;
                                case ("mod"):
                                    dataService.AddMod(new Mod(name, path));
                                    break;
                            }
                            i += 3;
                        }
                        else
                        {
                            Console.WriteLine("Error: --add option requires three arguments: <type: iwad | sourceport | mod> <name> <path>.");
                            return;
                        }
                        break;
                }
            }
            if (showGreeting) { Console.WriteLine(Greeting); }
            if (showUsage) { Console.WriteLine(Usage); } 

            if (configPath != null && Path.Exists(configPath))
            {
                string jsonContent = File.ReadAllText(configPath);
                Models.GameProfile gameProfile = new Models.GameProfile();
                try { gameProfile = JsonSerializer.Deserialize<Models.GameProfile>(jsonContent); } 
                catch (Exception ex)
                {
                    Console.WriteLine($"Parsing exception: {ex.ToString()}");
                    return;
                }
                Console.WriteLine(gameProfile.Name);
                await new GameExecute().RunAsync(gameProfile);
            }
        }
    }
}
