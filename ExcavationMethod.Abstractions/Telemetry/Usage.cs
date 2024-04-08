using System;
using System.Collections.Generic;

namespace ExcavationMethod.Abstractions.Telemetry
{
    public static class Usage
    {
        private static IUsageTracker _client = new SilentUsageTracker();

        public static IUsageTracker Client 
        {
            get => _client;
            set => _client = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static void TrackEvent(
            string eventName,
            IDictionary<string, string>? properties = null,
            IDictionary<string, double>? metrics = null) =>
            _client.TrackEvent(eventName, properties, metrics);

        public static void TrackException(
            Exception exception, 
            IDictionary<string, string>? properties = null,
            IDictionary<string, double>? metrics = null) =>
            _client.TrackException(exception, properties, metrics);
    }
}
