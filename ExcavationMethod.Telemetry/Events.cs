using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExcavationMethod.Telemetry
{
    public static class Events
    {
        public static void TrackCustomEvent(
            this TelemetryClient client,
            string key,
            IDictionary<string, string>? properties = null,
            IDictionary<string, double>? metrics = null)
        {
            client.TrackEvent(key, properties, metrics);
        }
    }
}
