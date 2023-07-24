using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProtectApp.Core.Models;
using PublicAPI.Core.Interfaces;
using PublicAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PublicApi.Services
{
    public class AzureHttpService : IAzureHttpService
    {
        readonly AzureClientCredentialSettings _credentials;
        readonly AzureAdSettings _adSettings;
        public AzureHttpService(IOptions<AzureClientCredentialSettings> credentials, IOptions<AzureAdSettings> adSettings) {
            _credentials = credentials.Value;
            _adSettings = adSettings.Value;
        }

        public async Task<string> GetToken()
        {
            Uri requestUri = new Uri($"https://login.microsoftonline.com/{_adSettings.TenantId}/oauth2/v2.0/token");

            List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("client_id", _credentials.ClientId),
                new KeyValuePair<string, string>("scope", _credentials.Scope),
                new KeyValuePair<string, string>("grant_type", _credentials.GrantType),
                new KeyValuePair<string, string>("client_secret", _credentials.ClientSecret)
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUri)
            {
                Content = new FormUrlEncodedContent(content),
            };

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.SendAsync(request);

            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JsonConvert.DeserializeObject(responseContent);

            if (response.IsSuccessStatusCode)
            {
                // dynamic values need to be assigned before passing back
                return (string)responseObject.access_token;
            }
            else
            {
                // TODO: catch error then pass back
                throw new Exception("Error getting access token");
            }
        }
    }
}
