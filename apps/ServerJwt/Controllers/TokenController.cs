using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerJwt.Data;
using ServerJwt.DTO;
using ServerJwt.Services;

namespace ServerJwt.Controllers
{
[Route("api/[controller]")]
  [ApiController]
  public class TokenController : ControllerBase
  {
    private readonly UserContext _userContext;
    private readonly ITokenService _tokenService;

    public TokenController(UserContext userContext, ITokenService tokenService)
    {
      this._userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
      this._tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    [HttpPost]
    [Route("refresh")]
    public IActionResult Refresh(TokenApiModel tokenApiModel)
    {
      if (tokenApiModel is null)
        return BadRequest("Invalid client request");

      string accessToken = tokenApiModel.AccessToken ?? string.Empty;
      string refreshToken = tokenApiModel.RefreshToken ?? string.Empty;

      var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
      var username = principal.Identity?.Name; //this is mapped to the Name claim by default

      var user = _userContext.LoginModels?.SingleOrDefault(u => u.UserName == username);

      if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        return BadRequest("Invalid client request");

      var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
      var newRefreshToken = _tokenService.GenerateRefreshToken();

      user.RefreshToken = newRefreshToken;
      user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

      _userContext.SaveChanges();

      return Ok(new LoginResponse()
      {
        Token = newAccessToken,
        RefreshToken = newRefreshToken
      });
    }

    [HttpPost, Authorize]
    [Route("revoke")]
    public IActionResult Revoke()
    {
      var username = User.Identity?.Name;

      var user = _userContext.LoginModels?.SingleOrDefault(u => u.UserName == username);
      if (user == null) return BadRequest();

      user.RefreshToken = null;

      _userContext.SaveChanges();

      return NoContent();
    }
  }
}
