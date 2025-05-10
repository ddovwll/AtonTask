using System.Security.Claims;
using AtonTask.Application.Services;

namespace AtonTask.WebApi.Middlewares;

public class UserPermissionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var userService = context.RequestServices.GetRequiredService<UserService>();
        var userIdString = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userId = userIdString is null ? Guid.Empty : Guid.Parse(userIdString);
        if (userId == Guid.Empty || await userService.CheckUserPermissionAsync(userId, context.RequestAborted))
        {
            await next(context);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
        }
    }
}