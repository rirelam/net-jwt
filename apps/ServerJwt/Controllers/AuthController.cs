using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ServerJwt.Data;
using ServerJwt.DTO;
using ServerJwt.Services;

namespace ServerJwt.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly UserContext _userContext;
    private readonly ITokenService _tokenService;

    public AuthController(UserContext userContext, ITokenService tokenService)
    {
      _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
      _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel userToLogin)
    {
      if (userToLogin is null)
      {
        return BadRequest("Invalid client request");
      }

      var user = _userContext?.LoginModels?.FirstOrDefault(u =>
          (u.UserName == userToLogin.UserName) && (u.Password == userToLogin.Password));

      if (user is null)
        return Unauthorized();

      var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName??string.Empty),
            new Claim(ClaimTypes.Role, "Manager")
        };

      var accessToken = _tokenService.GenerateAccessToken(claims);
      var refreshToken = _tokenService.GenerateRefreshToken();

      user.RefreshToken = refreshToken;
      user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

      _userContext?.SaveChanges();

      return Ok(new LoginResponse
      {
        Token = accessToken,
        RefreshToken = refreshToken
      });
    }
  }
}
