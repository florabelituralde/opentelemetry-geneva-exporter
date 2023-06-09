// ***********************************************************************
// Assembly         : GenevaOpenTelemetryAppService.Logging
// Author           : fituralde
// Created          : 10-02-2022
//
// Last Modified By : fituralde
// Last Modified On : 03-28-2023
// ***********************************************************************
// <copyright file="GenevaLogger.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using GenevaOpenTelemetryAppService.Geneva;
using GenevaOpenTelemetryAppService.Logging.Interfaces;
using Microsoft.Extensions.Options;
using OpenTelemetry.Logs;

namespace GenevaOpenTelemetryAppService.Logging
{
    /* This code is not actively being used in this application
     * However, you can use it to create a custom logger using Open telemetry SDK that exports to Geneva
     * You would also have to initialize this in startup.cs instead of the code in line 94-108, which is an auto logs collector that uses 
     * Microsoft.Extensions.Logging.ILogger interface */
    public class GenevaLogger : IGenevaLogger
    {
        private ILogger _logger;

        public GenevaLogger(IOptions<GenevaLoggingSettings> settings, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(settings.Value.ConnString))
                throw new ArgumentNullException(nameof(settings.Value.ConnString));

            var loggerFactory = LoggerFactory.Create(builder => builder
                .AddOpenTelemetry(loggerOptions =>
                {
                    loggerOptions.AddGenevaLogExporter(exporterOptions =>
                    {
                        exporterOptions.ConnectionString = settings.Value.ConnString;

                        if (settings.Value.PrePopulatedFields != null)
                        {
                            exporterOptions.PrepopulatedFields = settings.Value.PrePopulatedFields;
                        }
                    });
                }));
            

            _logger = loggerFactory.CreateLogger<GenevaLogger>();
        }

        public void LogException(Exception exp)
        {
            _logger.LogError(exp, exp.Message);
        }

        public void LogEvent(string? message, params object[]? args)
        {
            // construct data in format that geneva is expecting
            _logger.LogInformation(message.ToString(), args.ToArray());
        }

    }
}
