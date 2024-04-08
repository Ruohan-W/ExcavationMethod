using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcavationMethod.Revit.Application
{
    public class ApplicationSettings
    {
        public string? RevitVersion { get; set; }
        public string? RevitSubVersion { get; set; }
        public bool IsUpdateAvailable { get; set; }
        public bool HasForcedUpdates { get; set; }
    }
}
