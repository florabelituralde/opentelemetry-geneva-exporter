// ***********************************************************************
// Assembly         : GenevaOpenTelemetryAppService.Logging.Interfaces
// Author           : fituralde
// Created          : 10-02-2022
//
// Last Modified By : fituralde
// Last Modified On : 12-30-2022
// ***********************************************************************
// <copyright file="IGenevaMetrics.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace GenevaOpenTelemetryAppService.Logging.Interfaces
{
    public interface IGenevaMetrics
    {
        void CountException(Exception ex, IGenevaLogger logger);

        void RecordLatency(IGenevaLogger logger);
    }
}
