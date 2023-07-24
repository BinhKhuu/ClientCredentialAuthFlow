using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicAPI.Core.Models
{
    public class AzureClientCredentialSettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string GrantType { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;

    }
}
