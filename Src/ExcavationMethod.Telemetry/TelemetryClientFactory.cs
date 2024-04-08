using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ExcavationMethod.Telemetry
{
    public static class TelemetryClientFactory
    {
        private static TelemetryClient? _telemetryClient;
        public static bool Enabled { get; set; } = true;

        public static TelemetryClient CreateClient(string connectionString, string applicationName,Func<IDictionary<string, string>> customPropertyFactory)
        {
            var config = new TelemetryConfiguration
            {
                ConnectionString = connectionString
            };
            config.TelemetryChannel.DeveloperMode = Debugger.IsAttached;

#if DEBUG_R23 || DEBUG_R24
            config.TelemetryChannel.DeveloperMode = true;
#endif

            var initializer = new CustomTelemetryInitializer(applicationName, customPropertyFactory);
            config.TelemetryInitializers.Add(initializer);

            _telemetryClient = new TelemetryClient(config);
            _telemetryClient.Context.Session.Id = Guid.NewGuid().ToString();
            _telemetryClient.Context.User.Id = Environment.UserName;
            _telemetryClient.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
            return _telemetryClient;
        }

        public static void SetUser(this TelemetryClient client, string user)
        {
            client.Context.User.AuthenticatedUserId = user;
        }

        public static void Flush()
        {
            _telemetryClient?.Flush();
        }
    }
}
