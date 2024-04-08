using System;
using System.Collections.Generic;

namespace ExcavationMethod.Abstractions.Telemetry
{
    internal sealed class SilentUsageTracker : IUsageTracker
    {
        public void TrackEvent(
            string eventName,
            IDictionary<string, string>? properties = null,
            IDictionary<string, double>? metrics = null) 
        { }

        public void TrackException(
            Exception exeption,
            IDictionary<string, string>? properties = null,
            IDictionary<string, double>? metrics = null) 
        { }
    }
}
