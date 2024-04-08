using System;
using System.Collections.Generic;

namespace ExcavationMethod.Abstractions.Telemetry
{
    public interface IUsageTracker
    {
        void TrackEvent(
            string eventName,
            IDictionary<string, string>? properties = null,
            IDictionary<string, double>? metrics = null
            );

        void TrackException(
            Exception exception,
            IDictionary<string, string>? properties = null,
            IDictionary<string, double>? metrics = null
            );
    }
}
