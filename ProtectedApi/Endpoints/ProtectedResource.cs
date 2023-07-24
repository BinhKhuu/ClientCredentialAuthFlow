using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using ProtectApp.Core.Interfaces;
using ProtectApp.Core.Models;
using Microsoft.Extensions.Options;

namespace ProtectedApi.Endpoints
{
    public class ProtectedResource
    {
        private readonly ILogger<ProtectedResource> _logger;
        private readonly ITokenValidator _tokenValidator;
        private readonly AzureAdSettings _azureAdSettings;
        private static IConfiguration configuration;

        public ProtectedResource(ITokenValidator tokenValidator, IOptions<AzureAdSettings> adSettings, ILogger<ProtectedResource> logger)
        {
            _tokenValidator = tokenValidator;
            _azureAdSettings = adSettings.Value;
            _logger = logger;
        }

        [FunctionName("ProtectedResource")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var claimsPrincipal = await _tokenValidator.ValidateTokenAsync(req, _azureAdSettings.Scope);

            if (!_tokenValidator.HasRightRolesAndScope(claimsPrincipal, _azureAdSettings.ScopeName, _azureAdSettings.Roles))
            {
                return new UnauthorizedResult();
            }

            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
