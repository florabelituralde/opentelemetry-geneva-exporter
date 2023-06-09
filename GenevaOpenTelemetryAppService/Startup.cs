// ***********************************************************************
// Assembly         : GenevaOpenTelemetryAppService
// Author           : fituralde
// Created          : 10-02-2022
//
// Last Modified By : fituralde
// Last Modified On : 03-28-2023
// ***********************************************************************
// <copyright file="Startup.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Diagnostics;
using System.Diagnostics.Metrics;
using Azure.Monitor.OpenTelemetry.Exporter;
using GenevaOpenTelemetryAppService.OpenTelemetry.Metrics;
using GenevaOpenTelemetryAppService.OpenTelemetry.Trace;

using OpenTelemetry.Exporter.Geneva;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace GenevaOpenTelemetryAppService
{
    public class Startup
    {
        public static Dictionary<string, object>? PrePopulatedFields { get; private set; }

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connString = this.Configuration.GetValue<string>("OpenTelemetryConnectionString");
            var AIconnectionString = this.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING");
            var genevaAccount = this.Configuration.GetValue<string>("MONITORING_HOTPATH_ACCOUNT");
            var genevaNamespace = this.Configuration.GetValue<string>("MONITORING_HOTPATH_NAMESPACE");
            var servicemeter = new Meter("GenevaOTService.OpenTelemetryMeter", "1.0");
            var traceActivityEnricher = this.GetTraceActivtyEnricher();

            PrePopulatedFields = this.Configuration.GetSection("PrePopulatedFields").GetChildren()
                                    .ToDictionary(x => x.Key, x => (object)x.Value);

            services.AddControllers();

            // This code adds OpenTelemetry instrumentation, logging, and metrics to an ASP.NET Core web application.
            services.AddOpenTelemetryTracing(builder =>
            {
                // Setting a trace sampler to always sample traces (AlwaysOnSampler).
                builder.SetSampler(new AlwaysOnSampler());
                // Sets up a resource builder to add identifying information about the application (service name and version).
                builder.SetResourceBuilder(ResourceBuilder
                                                .CreateDefault()
                                                .AddService("GenevaAppServiceOtel", serviceVersion: "1.0"));
                /* Adds instrumentation for ASP.NET Core, with the option to record exceptions 
                 * and enrich traces with additional information about HTTP requests and exceptions. */
                builder.AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.EnrichWithHttpRequest = (activity, httpRequest) =>
                    {
                        traceActivityEnricher.EnrichRequestTelemetry(activity, httpRequest);
                    };
                    options.EnrichWithException = (activity, exception) =>
                    {
                        traceActivityEnricher.EnrichExceptionTelemetry(activity, exception);
                    };
                });
                /* Adds instrumentation for HttpClient, with the option to enrich traces with 
                 * additional information about Dependency calls and exceptions. */
                builder.AddHttpClientInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.EnrichWithHttpWebRequest = (activity, httpWebRequest) =>
                    {
                        traceActivityEnricher.EnrichWebRequestTelemetry(activity, httpWebRequest);
                    };
                    options.EnrichWithException = (activity, exception) =>
                    {
                        traceActivityEnricher.EnrichExceptionTelemetry(activity, exception);
                    };
                });
                // Adds a metrics processor to collect and process metrics data.
                builder.AddProcessor(new MetricsProcessor(servicemeter));
                // Adds a console exporter to output traces to the console.
                builder.AddConsoleExporter();
                // Adds a Jaeger exporter to export traces to Jaeger and see a dahsboard for distributed tracing.
                builder.AddJaegerExporter();
                // Adds a Geneva Trace exporter to export traces to geneva with a given connection string and pre-populated fields.
                builder.AddGenevaTraceExporter(options =>
                {
                    options.ConnectionString = connString;
                    options.PrepopulatedFields = PrePopulatedFields;
                });
                // Adds an Azure Monitor Trace Exporter to export traces to Application Insights.
                builder.AddAzureMonitorTraceExporter(options => options.ConnectionString = AIconnectionString);
            });

            services.AddLogging(builder =>
            {
                builder.AddConfiguration(this.Configuration);
                /* Adds OpenTelemetry logging, with the option to output logs to the console 
                 * and to Geneva with a given connection string and pre-populated fields. */
                builder.AddOpenTelemetry(otel =>
                {
                    otel.AddConsoleExporter();
                    otel.AddGenevaLogExporter(options =>
                    {
                        options.ConnectionString = connString;
                        options.PrepopulatedFields = PrePopulatedFields;
                    });
                });
            });

            services.AddOpenTelemetryMetrics(builder =>
            {
                // Adds OpenTelemetry metrics instrumentation for ASP.NET Core and HttpClient.
                builder.AddAspNetCoreInstrumentation();
                builder.AddHttpClientInstrumentation();
                // Adds a meter that serves as a listener to create metrics
                builder.AddMeter(servicemeter.Name);
                /* Adds a Geneva Metric Exporter to export metrics data to geneva
                 * with a given MDM Account/Namespace and pre-populated metric dimensions. */
                builder.AddGenevaMetricExporter(options =>
                {
                    options.ConnectionString = $"Account={genevaAccount};Namespace={genevaNamespace}";
                    options.PrepopulatedMetricDimensions = PrePopulatedFields;
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }

        public TraceActivityEnricher GetTraceActivtyEnricher()
        {
            var region = this.Configuration.GetValue<string>("MONITORING_REGION");
            var buildversion = this.Configuration.GetValue<string>("MONITORING_RELEASEVERSION");
            var webAppName = this.Configuration.GetValue<string>("MONITORING_WEBAPPNAME");

            var enricher = new TraceActivityEnricher(region, buildversion, webAppName);

            return enricher;
        }
    }
}
