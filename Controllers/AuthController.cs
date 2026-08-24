using Apex_Website_API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Apex_Website_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("token")]
        public IActionResult GenerateToken([FromBody] AuthRequest request)
        {

            _logger.LogInformation("AUTH | Token generation started"); 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Local testing credentials
            var validClientId = _configuration["Jwt:ClientId"];
            var validClientSecret = _configuration["Jwt:ClientSecret"];

            if (request.ClientId != validClientId || request.ClientSecret != validClientSecret)
            {
                _logger.LogWarning( "AUTH | Client authentication failed");
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid ClientId or ClientSecret."
                });
            }

            _logger.LogInformation("AUTH | Client authentication successful");
            var jwtKey = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expiryMinutes =Convert.ToInt32(_configuration["Jwt:ExpiryMinutes"]);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,request.ClientId),

                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));
            var credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                        (
                            issuer: issuer,
                            audience: audience,
                            claims: claims,
                            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                            signingCredentials: credentials);
            
            var tokenString =new JwtSecurityTokenHandler().WriteToken(token);
            _logger.LogInformation("AUTH | JWT token generated successfully | ExpiryMinutes: {ExpiryMinutes}",expiryMinutes);
            return Ok(new
            {
                success = true,
                token = tokenString,
                expiresInMinutes = expiryMinutes
            });
        }
    }
}