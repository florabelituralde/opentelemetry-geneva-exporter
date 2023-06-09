// ***********************************************************************
// Assembly         : GenevaOpenTelemetryAppService.OpenTelemetry.Trace
// Author           : fituralde
// Created          : 10-02-2022
//
// Last Modified By : fituralde
// Last Modified On : 03-28-2023
// ***********************************************************************
// <copyright file="TraceActivityEnricher.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace GenevaOpenTelemetryAppService.OpenTelemetry.Trace
{
    using System.Diagnostics;
    using System.Net;

    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Used for enriching the OpenTelemetry traces with additional information.
    /// These are used for adding dimensions to traces in <see cref="TraceActivityEnricher"/>.
    /// </summary>
    public class TraceActivityEnricher
    {
        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        private string Region { get; set; }

        /// <summary>
        /// Gets or sets the build version.
        /// </summary>
        private string BuildVersion { get; set; }

        /// <summary>
        /// Gets or sets the web app name.
        /// </summary>
        private string WebAppName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceActivityEnricher"/> class.
        /// </summary>
        /// <param name="region"/>
        /// <param name="buildversion"/>
        /// <param name="webappname"/>
        public TraceActivityEnricher(string region, string buildversion, string webappname)
        {
            this.Region = region;
            this.BuildVersion = buildversion;
            this.WebAppName = webappname;
        }

        /// <summary>
        /// Method used for enriching auto-instrumented HttpRequest or Request Telemetry with additional tags.
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="httpRequest"></param>
        public void EnrichRequestTelemetry(Activity? activity, HttpRequest httpRequest)
        {
            if (activity != null)
            {
                activity.SetTag(TraceConstants.StrRegionName, this.Region);
                activity.SetTag(TraceConstants.StrBuildVersion, this.BuildVersion);
                activity.SetTag(TraceConstants.StrWebAppName, this.WebAppName);
                activity.SetTag(TraceConstants.StrControllerName, activity.DisplayName);
                activity.SetTag(TraceConstants.StrOperationName, activity.OperationName);
                activity.SetTag(TraceConstants.StrEventName, (EventName)activity.Kind);
                activity.SetTag(TraceConstants.StrSpanKind, (SpanKind)activity.Kind);
            }
        }

        /// <summary>
        /// Method used for enriching auto-instrumented HttpWebRequest or Dependency Calls with additional tags.
        /// </summary>
        /// <param name="activity"/>
        /// <param name="httpWebRequest"/>
        public void EnrichWebRequestTelemetry(Activity? activity, HttpWebRequest httpWebRequest)
        {
            if (activity != null)
            {
                activity.SetTag(TraceConstants.StrRegionName, this.Region);
                activity.SetTag(TraceConstants.StrBuildVersion, this.BuildVersion);
                activity.SetTag(TraceConstants.StrWebAppName, this.WebAppName);
                activity.SetTag(TraceConstants.StrControllerName, activity.DisplayName);
                activity.SetTag(TraceConstants.StrOperationName, activity.OperationName);
                activity.SetTag(TraceConstants.StrEventName, (EventName)activity.Kind);
                activity.SetTag(TraceConstants.StrSpanKind, (SpanKind)activity.Kind);
            }
        }

        /// <summary>
        /// Method used for enriching auto-instrumented ExceptionTelemetry with additional tags.
        /// </summary>
        /// <param name="activity"/>
        /// <param name="exceptionTelemetry"/>
        public void EnrichExceptionTelemetry(Activity? activity, Exception exceptionTelemetry)
        {
            if (activity != null)
            {
                activity.SetTag(TraceConstants.StrExceptionType, exceptionTelemetry.GetType().ToString());
                activity.SetTag(TraceConstants.StrExceptionMessage, exceptionTelemetry.Message);
                activity.SetTag(TraceConstants.StrExceptionSource, exceptionTelemetry.Source);
                activity.SetTag(TraceConstants.StrStackTrace, exceptionTelemetry.StackTrace);
                activity.SetTag(TraceConstants.StrTargetSite, exceptionTelemetry.TargetSite);
                activity.SetTag(TraceConstants.StrControllerName, activity.DisplayName);
                activity.SetTag(TraceConstants.StrOperationName, activity.OperationName);
                activity.SetTag(TraceConstants.StrEventName, EventName.ExceptionTelemetry);
                activity.SetTag(TraceConstants.StrSpanKind, (SpanKind)activity.Kind);
            }
        }
    }
}
