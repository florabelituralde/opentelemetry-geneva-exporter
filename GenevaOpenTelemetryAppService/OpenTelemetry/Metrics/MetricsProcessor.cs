// ***********************************************************************
// Assembly         : GenevaOpenTelemetryAppService.OpenTelemetry.Metrics
// Author           : fituralde
// Created          : 10-02-2022
//
// Last Modified By : fituralde
// Last Modified On : 03-28-2023
// ***********************************************************************
// <copyright file="MetricsProcessor.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace GenevaOpenTelemetryAppService.OpenTelemetry.Metrics
{
    using System.Diagnostics;
    using System.Diagnostics.Metrics;

    using GenevaOpenTelemetryAppService.OpenTelemetry.Trace;

    using global::OpenTelemetry;

    /// <summary>
    /// Implementation of metrics collection following System Diagnostics API.
    /// </summary>
    public class MetricsProcessor : BaseProcessor<Activity>
    {
        /// <summary>
        /// A meter to create and track instruments and used for service metrics.
        /// </summary>
        private readonly Meter _metricsMeter;

        /// <summary>
        /// Counter to count every request that is coming into the service.
        /// </summary>
        private readonly Counter<long> _requestCounter;

        /// <summary>
        /// Histogram to capture the duration of each request.
        /// </summary>
        private readonly Histogram<double> _requestDuration;

        private readonly List<string> statuscodeforreliablity = new List<string>() { "400", "401", "403", "404", "405", "200" };

        public MetricsProcessor(Meter metricsMeter)
        {
            this._metricsMeter = metricsMeter ?? throw new ArgumentNullException(nameof(metricsMeter));

            string moduleName = metricsMeter.Name.Split('.').First();

            this._requestCounter = this._metricsMeter.CreateCounter<long>($"RequestsCount_{moduleName}");
            this._requestDuration = this._metricsMeter.CreateHistogram<double>($"Latency_{moduleName}");
        }

        public override void OnStart(Activity data)
        {
            // Add relevant metrics dimensions here to be added on the start of each activity if needed
        }

        public override void OnEnd(Activity data)
        {
            // The custom dimensions are retrieved from the tags set by the <see cref="TraceAcitivityEnricher"/>.
            string controller = data?.DisplayName ?? string.Empty;
            string operationName = data?.OperationName ?? string.Empty;
            var metricType = (MetricName)data.Kind;

            // Some activity tags are provided out-of-the-box by OpenTelemetry.
            string statusCode = data?.GetTagItem("http.status_code")?.ToString() ?? string.Empty;
            var reliability = this.statuscodeforreliablity.Contains(statusCode) ? 100 : 0;

            // Record the request duration.
            this.RecordLatency(data?.Duration.TotalMilliseconds ?? 0, controller, operationName, statusCode, metricType, reliability);

            // Count the number of incoming requests.
            this.CountRequest(controller, operationName, statusCode, metricType, reliability);

            data?.AddTag(TraceConstants.StrReliability, reliability);

            // Stop the activity, so that the duration can be recorded as a tag.
            data?.Stop();

            // Add the duration stored in the activity as a tag.
            data?.AddTag(TraceConstants.StrLatency, data.Duration.TotalMilliseconds);
        }

        public void CountRequest(string controller, string operationName, string statusCode, MetricName metricName, int reliability)
        {
            var dimensions = new KeyValuePair<string, object?>[]
                                 {
                                     new KeyValuePair<string, object?>(TraceConstants.StrControllerName, controller),
                                     new KeyValuePair<string, object?>(TraceConstants.StrOperationName, operationName),
                                     new KeyValuePair<string, object?>(TraceConstants.StrStatusCode, statusCode),
                                     new KeyValuePair<string, object?>(TraceConstants.StrMetricName, metricName),
                                     new KeyValuePair<string, object?>(TraceConstants.StrReliability, reliability)
                                 };
            this._requestCounter.Add(1, dimensions);
        }

        public void RecordLatency(double durationInMilliseconds, string controller, string operationName, string statusCode, MetricName metricName, int reliability)
        {
            var dimensions = new KeyValuePair<string, object?>[]
                                 {
                                     new KeyValuePair<string, object?>(TraceConstants.StrControllerName, controller),
                                     new KeyValuePair<string, object?>(TraceConstants.StrOperationName, operationName),
                                     new KeyValuePair<string, object?>(TraceConstants.StrStatusCode, statusCode),
                                     new KeyValuePair<string, object?>(TraceConstants.StrMetricName, metricName),
                                     new KeyValuePair<string, object?>(TraceConstants.StrReliability, reliability)
                                 };
            this._requestDuration.Record(durationInMilliseconds, dimensions);
        }
    }
}
