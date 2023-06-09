// ***********************************************************************
// Assembly         : GenevaOpenTelemetryAppService.Logging.Interfaces
// Author           : fituralde
// Created          : 10-02-2022
//
// Last Modified By : fituralde
// Last Modified On : 12-30-2022
// ***********************************************************************
// <copyright file="IGenevaLogger.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace GenevaOpenTelemetryAppService.Logging.Interfaces
{
    public interface IGenevaLogger
    {
        void LogException(Exception exp);
        void LogEvent(string? message, params object[]? args);
    }
}
