using System;
using System.Collections.Generic;
using System.Text;

namespace ExcavationMethod.Telemetry
{
    public static class TelemetryConfig
    {
#if DEBUG_R23 || DEBUG_R24
        public const string ConnectionString = "";
#else
        public const string ConnectionString = "";
#endif
    }
}
