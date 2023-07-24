using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicAPI.Core.Interfaces
{
    public interface IAzureHttpService
    {
        public Task<string> GetToken();

    }
}
