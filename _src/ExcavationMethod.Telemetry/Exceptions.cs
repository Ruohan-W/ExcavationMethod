using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExcavationMethod.Telemetry
{
    public static class Exceptions
    {
        public static void TrackCustomException(this TelemetryClient client, Exception ex)
        {
            if(ex != null)
            {
                var telEx = new Microsoft.ApplicationInsights.DataContracts.ExceptionTelemetry(ex);
                client.TrackException(telEx);
                client.Flush();
            }
        }
    }
}
