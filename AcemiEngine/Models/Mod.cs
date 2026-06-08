using System;
using System.Collections.Generic;
using System.Text;

namespace AcemiEngine.Models
{
    internal class Mod
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public Mod(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }
    }
}
