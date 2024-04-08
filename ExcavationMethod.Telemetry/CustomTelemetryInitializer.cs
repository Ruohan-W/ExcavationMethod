using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ExcavationMethod.Telemetry
{
    public class CustomTelemetryInitializer : ITelemetryInitializer
    {
        private readonly string _serviceName;
        private readonly Func<IDictionary<string, string>> _customPropertyFactory;

        public CustomTelemetryInitializer(string serviceName, Func<IDictionary<string, string>> customPropertyFactory)
        {
            _serviceName = serviceName;
            _customPropertyFactory = customPropertyFactory;
        }

        [DebuggerStepThrough]
        public void Initialize(ITelemetry telemetry)
        {
            if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
            {
                telemetry.Context.Cloud.RoleName = _serviceName;
            }

            // No need to continue if we cannot add custom properties to the telemetry.
            if (telemetry is not ISupportProperties telemetryWithProperties) return;

            foreach (var property in _customPropertyFactory())
            {
                telemetryWithProperties.Properties[property.Key] = property.Value;
            }
        }
    }
}
