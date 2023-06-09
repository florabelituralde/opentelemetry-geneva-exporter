// ***********************************************************************
// Assembly         : GenevaOpenTelemetryAppService.Logging
// Author           : fituralde
// Created          : 10-02-2022
//
// Last Modified By : fituralde
// Last Modified On : 03-28-2023
// ***********************************************************************
// <copyright file="GenevaTracer.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using GenevaOpenTelemetryAppService.Geneva;
using GenevaOpenTelemetryAppService.Logging.Interfaces;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Exporter.Geneva;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace GenevaOpenTelemetryAppService.Logging
{
    /* This code is not actively being used in this application
     * However, you can use it to create custom traces instead of using the auto trace collector
     * on startup.cs from line 42-92 
     * Using the custom sdk might also cause some inconsistency with traces,logs,metrics correlation
     * Please make sure to instrument your code to get current activity to correlate traces properly*/
    public class GenevaTracer : IGenevaTracer
    {

        public GenevaTracer(IOptions<GenevaLoggingSettings> settings)
        {
            var tracerProvider = Sdk.CreateTracerProviderBuilder()
                 .SetSampler(new AlwaysOnSampler())
                 .SetResourceBuilder(ResourceBuilder.CreateDefault()
                 .AddService("GenevaOTDemoService"))
                 .AddAspNetCoreInstrumentation()
                 .AddHttpClientInstrumentation()
                 .AddSource("GenevaOT.Demo")
                 .AddGenevaTraceExporter(options =>
                 {
                     options.ConnectionString = settings.Value.ConnString;

                     if (settings.Value.PrePopulatedFields != null)
                     {
                         options.PrepopulatedFields = settings.Value.PrePopulatedFields;
                     }

                 })
                .Build();

            tracerProvider?.Dispose();
        }

        public void TraceDependencyTelemetry(ActivitySource activitySource, IGenevaLogger logger)
        {
            using (var activity = activitySource.StartActivity("DependencyName", ActivityKind.Client))
            {
                logger.LogEvent("TraceDependencyTelemetry traced an activity.");

                activity?.SetTag("httpStatusCode", 200);
                activity?.SetTag("dbStatement", "SqlStatement");
                // Use httpUrl for HttpCalls.
                // activity?.SetTag("httpUrl", "HttpUrl");
                activity?.SetTag("customprop1", "custom value1");
                activity?.SetTag("customprop2", "custom value2");
                activity?.SetStatus(ActivityStatusCode.Error, "Error");

                TraceRequestTelemetry(activitySource, logger);
            }
        }

        public void TraceRequestTelemetry(ActivitySource activitySource, IGenevaLogger logger)
        {
            using (var activity = activitySource.StartActivity("RequestName", ActivityKind.Server))
            {
                logger.LogEvent("TraceRequestTelemetry traced an activity.");

                activity?.SetTag("httpStatusCode", 200);
                activity?.SetTag("httpUrl", "http://example.com");
                activity?.SetTag("customprop1", "custom value1");
                activity?.SetTag("customprop2", "custom value2");
            }
        }
    }
}
