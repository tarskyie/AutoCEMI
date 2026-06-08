using AcemiEngine.Models;
using Microsoft.Data.Sqlite;
using System.Text.Json;

namespace AcemiEngine.Services
{
    internal class DataService
    {
        public string persistentDataDirectoryPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Roaming\\AutoCEMI";
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
            string json = System.Text.Json.JsonSerializer.Serialize(profile);
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
                        ports.Add(new SourcePort(name: reader.GetString(0), path: reader.GetString(1)));
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
                foreach (var port in ports)
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

        public List<SourcePort> ports = new();
        public List<Iwad> iwads = new();
        public List<Mod> mods = new();

        public void AddPort(SourcePort port) { ports.Add(port); SaveData(); }
        public void AddIwad(Iwad iwad) { iwads.Add(iwad); SaveData(); }
        public void AddMod(Mod mod) { mods.Add(mod); SaveData(); }
        public void RemovePort(SourcePort port) { ports.Remove(port); SaveData(); }
        public void RemoveIwad(Iwad iwad) { iwads.Remove(iwad); SaveData(); }
        public void RemoveMod(Mod mod) { mods.Remove(mod); SaveData(); }

        public void UpdatePlayertime(TimeSpan delta)
        {
            UserProfile? profile = JsonSerializer.Deserialize<UserProfile>(File.ReadAllText($"{persistentDataDirectoryPath}//{profileFileName}"));
            profile?.PlayTime += delta;
        }
    }
}
