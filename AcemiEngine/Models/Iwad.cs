using System;
using System.Collections.Generic;
using System.Text;

namespace AcemiEngine.Models
{
    internal class Iwad
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public Iwad(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }
    }
}
