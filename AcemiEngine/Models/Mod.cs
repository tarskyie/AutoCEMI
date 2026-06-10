using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCEMI.Models
{
    public class Mod
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
