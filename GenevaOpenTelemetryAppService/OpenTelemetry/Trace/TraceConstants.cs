// ***********************************************************************
// Assembly         : GenevaOpenTelemetryAppService.OpenTelemetry.Trace
// Author           : fituralde
// Created          : 10-02-2022
//
// Last Modified By : fituralde
// Last Modified On : 12-30-2022
// ***********************************************************************
// <copyright file="TraceConstants.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace GenevaOpenTelemetryAppService.OpenTelemetry.Trace
{
    /// <summary>Constants used for Open Telemetry dimensions.</summary>
    public static class TraceConstants
    {
        // Required Telemetry tags
        public const string StrRegionName = "RegionName";

        public const string StrBuildVersion = "BuildVersion";

        public const string StrWebAppName = "WebAppName";

        public const string StrSpanKind = "SpanKind";

        public const string StrEventName = "EventName";

        public const string StrControllerName = "Controller";

        public const string StrOperationName = "OperationName";

        // Exception Telemetry tags
        public const string StrExceptionType = "ExceptionType";

        public const string StrExceptionMessage = "ExceptionMessage";

        public const string StrExceptionSource = "Source";

        public const string StrStackTrace = "StackTrace";

        public const string StrTargetSite = "TargetSite";

        // Metrics Telemetry dimensions and tags
        public const string StrReliability = "Reliability";

        public const string StrStatusCode = "StatusCode";

        public const string StrMetricName = "MetricType";

        public const string StrLatency = "Latency";
    }

    public enum EventName
    {
        RequestTelemetry = 1,
        DependencyTelemetry = 2,
        ExceptionTelemetry
    }

    public enum MetricName
    {
        RequestMetrics = 1,
        DependencyMetrics = 2,
        ExceptionMetrics,
    }

    public enum SpanKind
    {
        INTERNAL = 0,
        SERVER = 1,
        CLIENT = 2,
        PRODUCER = 3,
        CONSUMER = 4,
    }

}
