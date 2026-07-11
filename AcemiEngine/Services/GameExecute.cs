using AutoCEMI.Models;
using System.Diagnostics;

namespace AutoCEMI.Services
{
    public class GameExecute
    {
        private DataService dataService = new();
        public async Task RunAsync(GameProfile gameProfile, ExecutionOptions? executionOptions = null)
        {
            string sourcePortPath = gameProfile.Game.SourcePortPath;
            List<string> engineArgs = new();

            engineArgs.Add(gameProfile.Game.CustomArguments);

            for (int i = 0; i < gameProfile.Mods.LoadOrder.Count; i++)
            {
                var mod = dataService.mods.FirstOrDefault(m => m.Name == gameProfile.Mods.LoadOrder[i]);
                if (mod != null)
                {
                    engineArgs.Add($"-file \"{mod.Path}\"");
                }
            }

            if (executionOptions != null)
            {
                if (!string.IsNullOrWhiteSpace(executionOptions.Map))
                {
                    if (executionOptions.Map[0] == 'E')
                    {
                        string[] indexes = executionOptions.Map.Replace("E", string.Empty).Split('M').ToArray();
                        if (indexes.Length >= 2) engineArgs.Add($"-warp {indexes[0]} {indexes[1]}");
                    }
                    else
                    {
                        engineArgs.Add($"-warp {executionOptions.Map.Replace("MAP", string.Empty)}");
                    }
                }

                if (executionOptions.Difficulty >= 0) engineArgs.Add($"-skill {executionOptions.Difficulty + 1}");
            }

            // look up source port and iwad path from data service
            var sourcePort = dataService.sourcePorts.FirstOrDefault(p => p.Name == gameProfile.Game.SourcePort);
            if (sourcePort != null)
            {
                sourcePortPath = sourcePort.Path;
            }
            var wad = dataService.iwads.FirstOrDefault(w => w.Name == gameProfile.Game.Iwad.Trim());
            if (wad != null)
            {
                engineArgs.Add($"-iwad \"{wad.Path}\"");
            }

            if (!File.Exists(sourcePortPath))
            {
                return;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = sourcePortPath,
                Arguments = string.Join(" ", engineArgs),
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process? gameProcess;
            try
            {
                gameProcess = Process.Start(startInfo);
            }
            catch { return; }
            if (gameProcess == null)
            {
                return;
            }
            PlaytimeTracker tracker = new PlaytimeTracker();
            TimeSpan deltaTime = await tracker.StartTracking(gameProcess.ProcessName);
            dataService.UpdatePlayertime(deltaTime);
        }
    }
}
