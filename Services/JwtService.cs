using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TokenAuth.Data;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TokenAuth.Services.Interfaces;

namespace TokenAuth.Services
{
    public class JwtService : IJwtService
    {
        protected readonly UserManager<ApplicationUser> _userManager;

        public JwtService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // TODO: get from configuration...
        public static SigningCredentials GetSigningCredentials()
        {
            return new SigningCredentials(GetSecurityKey(), SecurityAlgorithms.HmacSha256);
        }

        public static SymmetricSecurityKey GetSecurityKey()
        {
            var keyByteArray = Encoding.ASCII.GetBytes("dfasdfasdfasdfasdafasdfasdfasdfasfasdf");
            return new SymmetricSecurityKey(keyByteArray);
        }
        
        public static string Issuer { get { return "TestIssuer"; } }
        public static string Audience { get { return "TestAudience"; } }
        public static DateTime? Expires { get { return DateTime.UtcNow.AddMinutes(10); } }

        public async Task<JwtSecurityToken> GetJwtSecurityToken(ApplicationUser user)
        {
            return new JwtSecurityToken(
                // issuer: _appConfiguration.Value.SiteUrl,
                // audience: _appConfiguration.Value.SiteUrl,
                // issuer: "http://localhost:5000",
                // audience: "http://localhost:5000",
                issuer: Issuer,
                audience: Audience,
                claims: await GetTokenClaims(user),
                expires: Expires,
                signingCredentials: GetSigningCredentials()
            );
        }

        protected async Task<IEnumerable<Claim>> GetTokenClaims(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var tokenClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName)
            };

            return tokenClaims.Union(userClaims);
        }
    }
}