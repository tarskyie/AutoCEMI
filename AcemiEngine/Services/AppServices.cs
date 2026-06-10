using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCEMI.Services
{
    public class AppServices
    {
        public static DataService DataService { get; } = new DataService();
        public static GameExecute GameExecute { get; } = new GameExecute();
        public static PlaytimeTracker PlaytimeTracker { get; } = new PlaytimeTracker();
    }
}
