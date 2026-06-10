using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AutoCEMI.Services
{
    internal class PlaytimeTracker
    {
        private Process? gameProcess;
        private TimeSpan accumulatedTime = TimeSpan.Zero;
        private DateTime? sessionStart = null;

        public async Task<TimeSpan> StartTracking(string processName)
        {
            while (true)
            {
                // Try to find the process
                var processes = Process.GetProcessesByName(processName);

                if (processes.Length > 0)
                {
                    gameProcess = processes[0];
                    sessionStart = DateTime.Now;

                    Console.WriteLine("Game started!");
                    Console.WriteLine("Tracking playtime...");

                    // Wait for the process to exit
                    await Task.Run(() => gameProcess.WaitForExit());

                    // Add elapsed time
                    if (sessionStart.HasValue)
                    {
                        accumulatedTime += DateTime.Now - sessionStart.Value;
                        sessionStart = null;
                    }

                    Console.WriteLine($"Game closed. Total time: {accumulatedTime}");
                    return accumulatedTime;
                }

                await Task.Delay(1000); // Check every second
            }
        }
    }
}
