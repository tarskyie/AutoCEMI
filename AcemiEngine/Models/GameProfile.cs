using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace AcemiEngine.Models
{
    public class GameProfile
    {
        [JsonPropertyName("profile_version")]
        public string ProfileVersion { get; set; } = string.Empty;

        [JsonPropertyName("profile_id")]
        public string ProfileId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("created")]
        public string Created { get; set; } = string.Empty;

        [JsonPropertyName("last_modified")]
        public string LastModified { get; set; } = string.Empty;

        [JsonPropertyName("game")]
        public GameSettings Game { get; set; } = new();

        [JsonPropertyName("mods")]
        public ModsSettings Mods { get; set; } = new();

        [JsonPropertyName("port_settings")]
        public PortSettings PortSettings { get; set; } = new();
    }

    public class GameSettings
    {
        [JsonPropertyName("iwad")]
        public string Iwad { get; set; } = string.Empty;

        [JsonPropertyName("source_port")]
        public string SourcePort { get; set; } = string.Empty;

        [JsonPropertyName("source_port_path")]
        public string SourcePortPath { get; set; } = string.Empty;

        [JsonPropertyName("compatibility_level")]
        public string CompatibilityLevel { get; set; } = string.Empty;

        [JsonPropertyName("custom_arguments")]
        public List<string> CustomArguments { get; set; } = new();
    }

    public class ModsSettings
    {
        [JsonPropertyName("load_order")]
        public List<string> LoadOrder { get; set; }= new();

        [JsonPropertyName("auto_sort")]
        public bool AutoSort { get; set; }
    }

    public class PortSettings
    {
        [JsonPropertyName("video")]
        public VideoSettings Video { get; set; } = new();

        [JsonPropertyName("audio")]
        public AudioSettings Audio { get; set; } = new();

        [JsonPropertyName("gameplay_flags")]
        public Dictionary<string, bool> GameplayFlags { get; set; } = new();
    }

    public class VideoSettings
    {
        [JsonPropertyName("renderer")]
        public string Renderer { get; set; } = string.Empty;

        [JsonPropertyName("vsync")]
        public bool VSync { get; set; }

        [JsonPropertyName("fps_cap")]
        public int FpsCap { get; set; }

        [JsonPropertyName("resolution")]
        public string Resolution { get; set; } = string.Empty;
    }

    public class AudioSettings
    {
        [JsonPropertyName("sample_rate")]
        public int SampleRate { get; set; }

        [JsonPropertyName("backend")]
        public string Backend { get; set; } = string.Empty;
    }
}
