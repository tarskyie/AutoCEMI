namespace AutoCEMI.Models
{
    public class SourcePort
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public SourcePort(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }
    }
}
