using ExcavationMethod.Abstractions.Telemetry;
using Microsoft.ApplicationInsights;

namespace ExcavationMethod.Revit.Application
{
    public class UsageTracker : IUsageTracker
    {
        private readonly TelemetryClient _client;
        public UsageTracker(TelemetryClient client) => _client = client;
        public void TrackEvent(string eventName, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null)
        {
            _client.TrackEvent(eventName, properties, metrics);
        }

        public void TrackException(Exception exception, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null)
        {
            _client.TrackException(exception, properties, metrics);
        }
    }
}