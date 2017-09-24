# JWT Setup

https://blogs.msdn.microsoft.com/webdev/2017/04/06/jwt-validation-and-authorization-in-asp-net-core/

## Required Packages

````
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
````

## Modify Startup.cs

Include the following using statements:

````
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Text;
````

Modify the ConfigureServices method to include the following:

````
services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // options.Authority = "http://localhost:5000/";
    options.Audience = "TestAudience";
    options.RequireHttpsMetadata = false;   // for dev, not production

    var keyByteArray = Encoding.ASCII.GetBytes("dfasdfasdfasdfasdafasdfasdfasdfasfasdf");
    var signingKey = new SymmetricSecurityKey(keyByteArray);
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ValidateIssuer = true,
        ValidIssuer = "TestIssuer",
        ValidAudience = "TestAudience",
    };
});
````

## Issue Tokens in API

The following code will need to be added in a controller:

````
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
````

````        
/******************************* start jwt stuff... *******************************/

[HttpPost("token")]
public async Task<IActionResult> Token([FromBody] LoginModel model)
{
    if (!ModelState.IsValid)
    {
        return BadRequest();
    }

    var user = await _userManager.FindByNameAsync(model.Email);

    // if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) != PasswordVerificationResult.Success)
    // {
    //     return BadRequest();
    // }

    var token = await GetJwtSecurityToken(user);

    return Ok(new
    {
        token = new JwtSecurityTokenHandler().WriteToken(token),
        expiration = token.ValidTo
    });
}

private async Task<JwtSecurityToken> GetJwtSecurityToken(ApplicationUser user)
{
    var userClaims = await _userManager.GetClaimsAsync(user);

    var keyByteArray = Encoding.ASCII.GetBytes("dfasdfasdfasdfasdafasdfasdfasdfasfasdf");
    var signingKey = new SymmetricSecurityKey(keyByteArray);
    return new JwtSecurityToken(
        issuer: "TestIssuer",
        audience: "TestAudience",
        claims: GetTokenClaims(user).Union(userClaims),
        expires: DateTime.UtcNow.AddMinutes(10),
        signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
    );
}

private static IEnumerable<Claim> GetTokenClaims(ApplicationUser user)
{
    return new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName)
    };
}

/******************************* end jwt stuff... *******************************/
````

## Requiring Tokens for API Endpoints

The API action methods will need to be decorated with the following attribute:

````
[Authorize(AuthenticationSchemes = "Bearer")]
````