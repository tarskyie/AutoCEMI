using AutoCEMI.Models;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace AutoCEMI.Services
{
    public class DataService
    {
        public string persistentDataDirectoryPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\AutoCEMI";
        private string databaseFileName = "data.db";
        private string profileFileName = "profile.json";
        private string connectionString => $"Data Source={persistentDataDirectoryPath}\\{databaseFileName}";
        public DataService()
        {
            if (!Directory.Exists(persistentDataDirectoryPath))
            {
                Directory.CreateDirectory(persistentDataDirectoryPath);
            }
            if (!File.Exists($"{persistentDataDirectoryPath}\\{databaseFileName}"))
            {
                CreateDatabase();
            }
            LoadData();

            if (!File.Exists($"{persistentDataDirectoryPath}\\{profileFileName}"))
            {
                CreateProfile();
            }
        }
        private void CreateProfile()
        {
            UserProfile profile = new();
            string json = System.Text.Json.JsonSerializer.Serialize(profile, AppJsonContext.Default.UserProfile);
            File.WriteAllText($"{persistentDataDirectoryPath}\\{profileFileName}", json);
        }
        private void CreateDatabase()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    CREATE TABLE IF NOT EXISTS SourcePorts (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Path TEXT NOT NULL
                    );
                    CREATE TABLE IF NOT EXISTS Iwads (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Path TEXT NOT NULL
                    );
                    CREATE TABLE IF NOT EXISTS Mods (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Path TEXT NOT NULL
                    );
                ";
                command.ExecuteNonQuery();
            }
            LoadData();
        }
        private void LoadData()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Name, Path FROM SourcePorts";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sourcePorts.Add(new SourcePort(name: reader.GetString(0), path: reader.GetString(1)));
                    }
                }
                command.CommandText = "SELECT Name, Path FROM Iwads";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        iwads.Add(new Iwad(name: reader.GetString(0), path: reader.GetString(1)));
                    }
                }
                command.CommandText = "SELECT Name, Path FROM Mods";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mods.Add(new Mod(name: reader.GetString(0), path: reader.GetString(1)));
                    }
                }
            }
        }
        private void SaveData()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM SourcePorts";
                command.ExecuteNonQuery();
                foreach (var port in sourcePorts)
                {
                    command.CommandText = $"INSERT INTO SourcePorts (Name, Path) VALUES ('{port.Name}', '{port.Path}')";
                    command.ExecuteNonQuery();
                }
                command.CommandText = "DELETE FROM Iwads";
                command.ExecuteNonQuery();
                foreach (var iwad in iwads)
                {
                    command.CommandText = $"INSERT INTO Iwads (Name, Path) VALUES ('{iwad.Name}', '{iwad.Path}')";
                    command.ExecuteNonQuery();
                }
                command.CommandText = "DELETE FROM Mods";
                command.ExecuteNonQuery();
                foreach (var mod in mods)
                {
                    command.CommandText = $"INSERT INTO Mods (Name, Path) VALUES ('{mod.Name}', '{mod.Path}')";
                    command.ExecuteNonQuery();
                }
            }
        }
        public void ClearData()
        {
            sourcePorts.Clear();
            iwads.Clear();
            mods.Clear();

            SaveData();
        }

        public void ClearAllData()
        {
            ClearData();
            CreateProfile();
        }

        public void ResetPlaytime()
        {
            UpdatePlayertime(null);
        }

        public ObservableCollection<SourcePort> sourcePorts { get; set; } = new ObservableCollection<SourcePort>();
        public ObservableCollection<Iwad> iwads { get; set; } = new ObservableCollection<Iwad>();
        public ObservableCollection<Mod> mods { get; set; } = new ObservableCollection<Mod>();
        public List<SourcePort> portsList
        {
            get => new List<SourcePort>(sourcePorts);
            set
            {
                portsList = value;
            }
        }
        public List<Iwad> iwadsList
        {
            get => new List<Iwad>(iwads);
            set
            {
                iwadsList = value;
            }
        }
        public List<Mod> modsList
        {
            get => new List<Mod>(mods);
            set
            {
                modsList = value;
            }
        }

        public void AddPort(SourcePort port)
        {
            var idx = portsList.FindIndex(m => m.Name == port.Name);
            if (idx >= 0)
            {
                sourcePorts[idx] = port;
                return;
            }
            sourcePorts.Add(port);
            SaveData();
        }
        public void AddIwad(Iwad iwad)
        {
            var idx = iwadsList.FindIndex(m => m.Name == iwad.Name);
            if (idx >= 0)
            {
                iwads[idx] = iwad;
                return;
            }
            iwads.Add(iwad);
            SaveData();
        }
        public void AddMod(Mod mod)
        {
            var idx = modsList.FindIndex(m => m.Name == mod.Name);
            if (idx >= 0)
            {
                mods[idx] = mod;
                return;
            }
            mods.Add(mod);
            SaveData();
        }
        public void RemovePort(string portName)
        {
            var port = sourcePorts.FirstOrDefault(m => m.Name == portName);
            if (port == null)
                return;
            sourcePorts.Remove(port);
            SaveData();
        }
        public void RemoveIwad(string iwadName)
        {
            var iwad = iwads.FirstOrDefault(m => m.Name == iwadName);
            if (iwad == null) return;
            iwads.Remove(iwad);
            SaveData();
        }
        public void RemoveMod(string modName)
        {
            var mod = mods.FirstOrDefault(m => m.Name == modName);
            if (mod == null) return;
            mods.Remove(mod);
            SaveData();
        }

        public void UpdatePlayertime(TimeSpan? delta)
        {
            var profilePath = Path.Combine(persistentDataDirectoryPath, profileFileName);
            var profile = JsonSerializer.Deserialize<UserProfile>(File.ReadAllText(profilePath), AppJsonContext.Default.UserProfile)
                          ?? new UserProfile();
            if (delta.HasValue)
            {
                profile.PlayTime += delta.Value;
            }
            else
            {
                profile.PlayTime = TimeSpan.Zero;
            }
            File.WriteAllText(profilePath, JsonSerializer.Serialize(profile, AppJsonContext.Default.UserProfile));
        }
        public void DisplayStats()
        {
            UserProfile? profile = JsonSerializer.Deserialize<UserProfile>(File.ReadAllText($"{persistentDataDirectoryPath}//{profileFileName}"), AppJsonContext.Default.UserProfile);
            Console.WriteLine($"Total playtime: {profile?.PlayTime}");
        }

        public UserProfile GetProfile()
        {
            var profilePath = Path.Combine(persistentDataDirectoryPath, profileFileName);
            return JsonSerializer.Deserialize<UserProfile>(File.ReadAllText(profilePath), AppJsonContext.Default.UserProfile)
                   ?? new UserProfile();
        }
        public UserProfile Profile
        {
            get => GetProfile();
        }

        public string GetIwadPathByName(string name)
        {
            var result = iwads.FirstOrDefault(n => n.Name == name);
            if (result == null) return string.Empty;
            return result.Path;
        }
    }
}
