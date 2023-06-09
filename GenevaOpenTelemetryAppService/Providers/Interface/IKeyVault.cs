// ***********************************************************************
// Assembly         : GenevaOpenTelemetryAppService.Providers.Interface
// Author           : fituralde
// Created          : 10-02-2022
//
// Last Modified By : fituralde
// Last Modified On : 12-30-2022
// ***********************************************************************
// <copyright file="IKeyVault.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace GenevaOpenTelemetryAppService.Providers.Interface
{
    public interface IKeyVault
    {
        string GetSecret(string secretName, int retries, int retryIntervalSeconds = 5);
    }
}
