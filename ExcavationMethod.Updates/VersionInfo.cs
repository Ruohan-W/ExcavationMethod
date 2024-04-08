using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcavationMethod.Updates
{
    public class VersionInfo
    {
        public Version Version { get; }

        public string FilePath { get; }

        public VersionInfo(Version version, string filePath)
        {
            Version = version;
            FilePath = filePath; 
        }
    }
}
