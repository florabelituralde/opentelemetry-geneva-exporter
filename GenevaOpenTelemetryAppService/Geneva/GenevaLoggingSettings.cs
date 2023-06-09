// ***********************************************************************
// Assembly         : GenevaOpenTelemetryAppService.Geneva
// Author           : fituralde
// Created          : 10-02-2022
//
// Last Modified By : fituralde
// Last Modified On : 12-30-2022
// ***********************************************************************
// <copyright file="GenevaLoggingSettings.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace GenevaOpenTelemetryAppService.Geneva
{
    public class GenevaLoggingSettings
    {
        public string? ConnString { get; set; }

        public Dictionary<string, object>? PrePopulatedFields { get; set; }

        public string[]? ExclusionList { get; set; }

        public string? MetricAccount { get; set; }

        public string? MetricNamespace { get; set; }

        public string? MetricVersion { get; set; }
    }
}
