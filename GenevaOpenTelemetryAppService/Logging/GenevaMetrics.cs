// ***********************************************************************
// Assembly         : GenevaOpenTelemetryAppService.Logging
// Author           : fituralde
// Created          : 10-02-2022
//
// Last Modified By : fituralde
// Last Modified On : 03-28-2023
// ***********************************************************************
// <copyright file="GenevaMetrics.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using GenevaOpenTelemetryAppService.Geneva;
using GenevaOpenTelemetryAppService.Logging.Interfaces;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Exporter.Geneva;
using OpenTelemetry.Metrics;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace GenevaOpenTelemetryAppService.Logging
{
    /* This code is not actively being used in this application
     * However, you can use it to create custom metrics if needeed instead of using the metrics processor
     * in trace telemetry from startup.cs line 79 */
    public class GenevaMetrics : IGenevaMetrics
    {
        private static readonly Meter servicemeter = new("GenevaOTService.OpenTelemetryMeter", "1.0");

        private static readonly Counter<long> exceptionsCounter = servicemeter.CreateCounter<long>("exceptionsCount");
        private static Histogram<double> httpServerDuration { get; } = servicemeter.CreateHistogram<double>("http.server.duration");

        private static ObservableGauge<long> responseGauge { get; } = servicemeter.CreateObservableGauge("ProcessPrivateMemorySize", () => 
                    new Measurement<long>(Process.GetCurrentProcess().PrivateMemorySize64, new KeyValuePair<string, object?>("instance", Environment.MachineName)));

        public GenevaMetrics(IOptions<GenevaLoggingSettings> settings)
        {
            var genevaAccount = settings.Value.MetricAccount;
            var genevaNamespace = settings.Value.MetricNamespace;

            var meterProvider = Sdk.CreateMeterProviderBuilder()
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddMeter(servicemeter.Name)
                .AddGenevaMetricExporter(options =>
                {
                    options.ConnectionString = $"Account={genevaAccount};Namespace={genevaNamespace}";
                })
                .Build();
        }

        public void CountException(Exception ex, IGenevaLogger logger) 
        {
            logger.LogEvent("Exception was added to CountException metric");
            exceptionsCounter.Add(1, new ("exception_type", ex), new ("exception_source", ex.Source));
        }

        public void RecordLatency(IGenevaLogger logger)
        {
            logger.LogEvent("RecordLatency metric added a new server duration record");

            httpServerDuration.Record(50, new ("http.method", "POST"), new ("http.scheme", "https"));
            httpServerDuration.Record(100, new ("http.method", "GET"), new ("http.scheme", "https"));
        }

    }
}
