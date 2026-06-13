using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace AutoCEMI.GUI.Services
{
    internal class ValidatorsService
    {
        public bool IsValidPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;
            try
            {
                Path.GetFullPath(path);
                return true;
            }
            catch { return false; }
        }
    }
}
