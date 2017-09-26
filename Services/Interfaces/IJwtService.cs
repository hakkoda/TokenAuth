using System.Threading.Tasks;
using TokenAuth.Data;
using System.IdentityModel.Tokens.Jwt;

namespace TokenAuth.Services.Interfaces
{
    public interface IJwtService
    {
        Task<JwtSecurityToken> GetJwtSecurityToken(ApplicationUser user);
    }
}