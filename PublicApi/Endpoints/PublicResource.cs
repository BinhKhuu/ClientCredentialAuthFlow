using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using PublicAPI.Core.Interfaces;

namespace PublicApi.Endpoints
{
    public class PublicResource
    {

        readonly IAzureHttpService _httpService;

        public PublicResource(IAzureHttpService httpService)
        {
            _httpService = httpService;
        }

        [FunctionName("PublicResource")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string token = await _httpService.GetToken();
            var httpClient = new HttpClient();
            Uri protectRequestUri = new Uri("http://localhost:7136/api/ProtectedResource");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpRequestMessage protectRequest = new HttpRequestMessage(HttpMethod.Get, protectRequestUri);

            await httpClient.SendAsync(protectRequest);

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
