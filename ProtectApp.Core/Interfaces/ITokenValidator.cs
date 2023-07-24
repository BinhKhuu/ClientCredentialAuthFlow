using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ProtectApp.Core.Interfaces
{
    public interface ITokenValidator
    {
        void GetJwtFromHeader(HttpRequest req);
        bool HasRightRolesAndScope(ClaimsPrincipal claimsPrincipal, string scopeName, string[]? roles);
        Task<ClaimsPrincipal> ValidateTokenAsync(HttpRequest req, string audience = "");
    }
}
