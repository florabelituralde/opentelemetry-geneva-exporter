// ***********************************************************************
// Assembly         : GenevaOpenTelemetryAppService.Logging.Interfaces
// Author           : fituralde
// Created          : 10-02-2022
//
// Last Modified By : fituralde
// Last Modified On : 12-30-2022
// ***********************************************************************
// <copyright file="IGenevaTracer.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Diagnostics;

namespace GenevaOpenTelemetryAppService.Logging.Interfaces
{
    public interface IGenevaTracer
    {
        void TraceDependencyTelemetry(ActivitySource activitySource, IGenevaLogger logger);

        void TraceRequestTelemetry(ActivitySource activitySource, IGenevaLogger logger);
    }
}
