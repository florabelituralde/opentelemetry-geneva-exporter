// ***********************************************************************
// Assembly         : GenevaOpenTelemetryAppService.Controllers
// Author           : fituralde
// Created          : 10-02-2022
//
// Last Modified By : fituralde
// Last Modified On : 12-30-2022
// ***********************************************************************
// <copyright file="PingController.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace GenevaOpenTelemetryAppService.Controllers
{
    using GenevaOpenTelemetryAppService.Providers;

    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class PingController : ControllerBase
    {
        private static readonly HttpClient HttpClient = new();

        private readonly ILogger<PingController> _logger;

        private readonly IConfiguration _configuration;

        public PingController(ILogger<PingController> logger, IConfiguration configuration)
        {
            this._logger = logger;
            this._configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var res = HttpClient.GetStringAsync("https://httpstat.us/200").Result;
            var res1 = HttpClient.GetStringAsync("https://www.bing.com/").Result;

            var keyVault = this._configuration.GetValue<string>("KeyVaultUri");
            var secretName = this._configuration.GetValue<string>("StorageConnectionString");

            var kv = new KeyVault(keyVault, this._logger);
            var result = kv.GetSecret(secretName, 2);
            if (result != null)
            {
                this._logger.LogInformation("Key vault retrieved secret successfully");
            }

            this._logger.LogInformation("Ping Controller has successfully sent a log to geneva using Open Telemetry");
            this._logger.LogError(new Exception(), "PingController sent an exception");
            this._logger.LogInformation(res);
            this._logger.LogInformation(res1);

            return this.Ok("This api sent new logs, metrics, and traces to Geneva using Open Telemetry");
        }
    }
}