using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtonTask.WebApi.Controllers;

[ApiController]
[Authorize]
public class BaseApiController : ControllerBase
{
    protected Guid UserId
    {
        get
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim is null ? Guid.Empty : Guid.Parse(userIdClaim);
        }
    }

    protected string Login => User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
    
    protected string RefreshToken => HttpContext.Request.Cookies["refreshToken"] ?? string.Empty;
    
    protected Guid RefreshTokenId {
        get
        {
            var tokenId = HttpContext.Request.Cookies["refreshTokenId"];
            return tokenId is null ? Guid.Empty : Guid.Parse(tokenId);
        }
    }
}