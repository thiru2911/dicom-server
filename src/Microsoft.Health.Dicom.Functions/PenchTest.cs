// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using EnsureThat;

namespace Microsoft.Health.Dicom.Functions
{
    public static class PenchTest
    {
        [FunctionName("PenchTest")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            EnsureArg.IsNotNull(log, nameof(log));
            EnsureArg.IsNotNull(req, nameof(req));
            log.LogInformation("C# HTTP trigger function processed a request.");
            string name = req.Query["name"];
#pragma warning disable CA1508 // Avoid dead conditional code
            string responseMessage = string.IsNullOrWhiteSpace(name)
#pragma warning restore CA1508 // Avoid dead conditional code
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
