// ***********************************************************************
// Assembly         : GenevaOpenTelemetryAppService.Providers
// Author           : fituralde
// Created          : 10-02-2022
//
// Last Modified By : fituralde
// Last Modified On : 12-30-2022
// ***********************************************************************
// <copyright file="KeyVault.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using GenevaOpenTelemetryAppService.Providers.Interface;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace GenevaOpenTelemetryAppService.Providers
{
    public class KeyVault : IKeyVault
    {
        /// <summary>
        /// The key vault URI.
        /// </summary>
        private readonly string _keyVaultUri;

        /// <summary>
        /// The log.
        /// </summary>
        private readonly ILogger _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyVault" /> class.
        /// </summary>
        /// <param name="keyVaultUri">The key vault URI.</param>
        /// <param name="log">The log.</param>
        public KeyVault(string keyVaultUri, ILogger log)
        {
            this._keyVaultUri = keyVaultUri;
            this._log = log;
        }

        /// <summary>
        /// Gets the secret.
        /// </summary>
        /// <param name="secretName"/>
        /// <param name="retries"/>
        /// <param name="retryIntervalSeconds"/>
        /// <returns>Task&lt;System.String&gt;..</returns>
        public string GetSecret(string secretName, int retries, int retryIntervalSeconds = 5)
        {
            this._log?.LogInformation($"Getting {secretName} from KeyVault");
            for (int tries = 1; tries <= retries; tries++)
            {
                try
                {
                    if (tries > 1)
                    {
                        Thread.Sleep(1000 * retryIntervalSeconds);
                        retryIntervalSeconds = Convert.ToInt32(retryIntervalSeconds * 1.5);
                    }

                    var azureTokenProvider = new AzureServiceTokenProvider();
                    var keyVaultClient =
                        new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureTokenProvider.KeyVaultTokenCallback));
                    var secret = keyVaultClient.GetSecretAsync(this._keyVaultUri, secretName).GetAwaiter().GetResult();
                    this._log?.LogInformation($"Key vault successfully retrieved {secretName}");
                    return secret.Value;
                }
                catch (Exception ex)
                {
                    if (tries <= retries)
                    {
                        continue;
                    }
                    else
                    {
                        this._log?.LogError($"KeyVault failed to retrieve {secretName}", ex);
                        throw;
                    }
                }
            }

            return null;
        }
    }
}
