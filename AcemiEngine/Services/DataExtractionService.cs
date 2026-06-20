using System.Text;

namespace AutoCEMI.Services
{
    public struct WadHeader
    {
        public byte[] Identification; // 4 bytes
        public int NumLumps;
        public int InfoTableOfs;
    }

    public struct LumpRecord
    {
        public int FilePos;
        public int Size;
        public byte[] Name; // 8 bytes
    }

    public static class DataExtractionService
    {
        public static List<string> ExtractMapNames(string wadFilePath)
        {
            List<string> mapNames = new();
            try
            {
                using FileStream fs = new FileStream(wadFilePath, FileMode.Open, FileAccess.Read);
                using BinaryReader reader = new BinaryReader(fs);

                // Read header
                WadHeader header = new WadHeader
                {
                    Identification = reader.ReadBytes(4),
                    NumLumps = reader.ReadInt32(),
                    InfoTableOfs = reader.ReadInt32()
                };

                // Verify it's a valid WAD
                string id = Encoding.ASCII.GetString(header.Identification, 0, 4).TrimEnd('\0');
                if (id != "IWAD" && id != "PWAD")
                {
                    Console.Error.WriteLine("Error: Not a valid WAD file.");
                    return mapNames;
                }

                // Seek to lump info table
                fs.Seek(header.InfoTableOfs, SeekOrigin.Begin);

                // Read all lumps
                LumpRecord[] lumps = new LumpRecord[header.NumLumps];

                for (int i = 0; i < header.NumLumps; i++)
                {
                    lumps[i] = new LumpRecord
                    {
                        FilePos = reader.ReadInt32(),
                        Size = reader.ReadInt32(),
                        Name = reader.ReadBytes(8)
                    };
                }

                // Look for map markers
                for (int i = 0; i < header.NumLumps - 4; i++)
                {
                    if (lumps[i].Size != 0)
                        continue;

                    string name = GetLumpName(lumps[i]);
                    string next1 = GetLumpName(lumps[i + 1]);
                    string next2 = GetLumpName(lumps[i + 2]);
                    string next3 = GetLumpName(lumps[i + 3]);
                    string next4 = GetLumpName(lumps[i + 4]);

                    if (next1 == "THINGS" &&
                        next2 == "LINEDEFS" &&
                        next3 == "SIDEDEFS" &&
                        next4 == "VERTEXES")
                    {
                        mapNames.Add(name);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
            }
            return mapNames;
        }

        private static string GetLumpName(LumpRecord lump)
        {
            // Convert byte array to string, stopping at first null terminator
            int length = 0;
            while (length < 8 && lump.Name[length] != 0)
                length++;

            return Encoding.ASCII.GetString(lump.Name, 0, length);
        }
    }
}
