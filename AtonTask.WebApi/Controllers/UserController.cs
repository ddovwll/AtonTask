using AtonTask.Application.Contracts;
using AtonTask.Application.Services;
using AtonTask.Domain.Exceptions;
using AtonTask.WebApi.Models.Requests;
using AtonTask.WebApi.Models.Responses;
using AtonTask.WebApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtonTask.WebApi.Controllers;

public class UserController(
    UserService userService,
    RefreshService refreshService,
    CookieOptionsProvider cookieOptionsProvider,
    IJwtService jwtService,
    ILogger<UserController> logger
) : BaseApiController
{
    /// <summary>
    /// Вход по логину и паролю
    /// Данные от пользователя Admin: Admin Admin1234 
    /// </summary>
    [HttpPost("/user/login")]
    [AllowAnonymous]
    public async Task<IActionResult> LogInAsync(
        [FromBody] CredentialsRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var (accessToken, userId) =
                await userService.LogInAsync(request.Login, request.Password, cancellationToken);
            var refreshToken = await refreshService.CreateAsync(userId, cancellationToken);
            var cookieOptions = cookieOptionsProvider.GetRefreshOptions();
            HttpContext.Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
            HttpContext.Response.Cookies.Append("refreshTokenId", refreshToken.Id.ToString(), cookieOptions);

            return Ok(accessToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при выполнении входа: {ErrorMessage}", e.Message);
            return Unauthorized();
        }
    }

    /// <summary>
    /// Обновление токена
    /// </summary>
    [HttpGet("/user/refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshAsync(CancellationToken cancellationToken)
    {
        try
        {
            var refresh = await refreshService.GetByIdAsync(RefreshTokenId, cancellationToken);
            if (!string.Equals(refresh.Token, RefreshToken))
            {
                return Forbid();
            }

            var refreshToken =
                await refreshService.RefreshAsync(refresh.UserId, RefreshTokenId, RefreshToken, cancellationToken);
            var user = await userService.GetUserAsync(refresh.UserId, cancellationToken);

            var accessToken = jwtService.GenerateToken(user);

            var cookieOptions = cookieOptionsProvider.GetRefreshOptions();
            HttpContext.Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

            return Ok(accessToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при выполнении входа: {ErrorMessage}", e.Message);
            return Unauthorized();
        }
    }

    /// <summary>
    /// Создание пользователя
    /// </summary>
    [HttpPost("/user/register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RegisterAsync(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var userDto = request.ToDto();
            await userService.CreateAsync(userDto, Login, cancellationToken);
            return Created();
        }
        catch (ConflictException e)
        {
            logger.LogError(e, "Ошибка при выполнении регистрации: {ErrorMessage}", e.Message);
            return Conflict(e.Message);
        }
    }

    /// <summary>
    /// Изменение имени, пола или даты рождения пользователя
    /// </summary>
    [HttpPut("/user/{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        [FromBody] UpdateRequest request,
        Guid id,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await userService.ChangePersonalDataAsync(
                id,
                Login,
                request.Name,
                request.Gender,
                request.Birthday,
                cancellationToken);

            return NoContent();
        }
        catch (NotFoundException e)
        {
            logger.LogError(e, "Ошибка при выполнении обновления персональны данных: {ErrorMessage}", e.Message);
            return BadRequest(e.Message);
        }
        catch (AccessException e)
        {
            logger.LogError(e, "Ошибка при выполнении обновления персональны данных: {ErrorMessage}", e.Message);
            return Forbid();
        }
        catch (ArgumentException e)
        {
            logger.LogError(e, "Ошибка при выполнении обновления персональны данных: {ErrorMessage}", e.Message);
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Изменение пароля
    /// </summary>
    [HttpPut("/user/{id:guid}/change-password")]
    public async Task<IActionResult> UpdatePasswordAsync(
        [FromBody] ChangePasswordRequest password,
        Guid id,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await userService.UpdatePasswordAsync(id, password.Password, Login, cancellationToken);

            return NoContent();
        }
        catch (AccessException e)
        {
            logger.LogError(e, "Ошибка при выполнении обновления персональны данных: {ErrorMessage}", e.Message);
            return Forbid();
        }
        catch (NotFoundException e)
        {
            logger.LogError(e, "Ошибка при выполнении обновления персональны данных: {ErrorMessage}", e.Message);
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Изменение логина
    /// </summary>
    [HttpPut("/user/{id:guid}/change-login")]
    public async Task<IActionResult> UpdateLoginAsync(
        [FromBody] string login,
        Guid id,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await userService.UpdateLoginAsync(id, login, Login, cancellationToken);

            if (id != UserId)
            {
                return NoContent();
            }

            var user = await userService.GetUserAsync(id, cancellationToken);

            var accessToken = jwtService.GenerateToken(user);
            var refreshToken =
                await refreshService.RefreshAsync(UserId, RefreshTokenId, RefreshToken, cancellationToken);

            var cookieOptions = cookieOptionsProvider.GetRefreshOptions();
            HttpContext.Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

            return Ok(accessToken);
        }
        catch (ConflictException e)
        {
            logger.LogError(e, "Ошибка при обновлении логина: {ErrorMessage}", e.Message);
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Запрос списка всех активных пользователей
    /// </summary>
    [HttpGet("/user/active")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetActiveUsersAsync(CancellationToken cancellationToken)
    {
        var activeUsers = await userService.GetAllActiveAsync(cancellationToken);
        return Ok(activeUsers);
    }

    /// <summary>
    /// Запрос пользователя по логину
    /// </summary>
    [HttpGet("/user/{login}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserByLoginAsync(string login, CancellationToken cancellationToken)
    {
        var user = await userService.GetByLoginAsync(login, cancellationToken);
        var response = user.ToResponse();
        return Ok(response);
    }

    /// <summary>
    /// Запрос пользователя по логину и паролю
    /// </summary>
    [HttpPost("/user")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetByCredentialsAsync(
        [FromBody] CredentialsRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (Login != request.Login)
            {
                return Forbid();
            }

            var user = await userService.GetByCredentialsAsync(request.Login, request.Password, cancellationToken);
            return Ok(user);
        }
        catch (AccessException e)
        {
            logger.LogError(e, "Ошибка при выполнении получения пользователя по логину и паролю: {ErrorMessage}",
                e.Message);
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Запрос всех пользователей старше определённого возраста
    /// </summary>
    [HttpGet("/user")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOlderThanAsync(int age, CancellationToken cancellationToken)
    {
        try
        {
            var users = await userService.GetOlderThanAsync(age, cancellationToken);
            return Ok(users);
        }
        catch (ArgumentException e)
        {
            logger.LogError(e, "Ошибка при выполнении получения пользователей по возрасту: {ErrorMessage}", e.Message);
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Удаление пользователя по логину полное или мягкое
    /// </summary>
    [HttpDelete("/user/{login}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RevokeAsync(bool hard, string login, CancellationToken cancellationToken)
    {
        try
        {
            if (hard)
            {
                await userService.DeleteAsync(login, cancellationToken);
                return NoContent();
            }

            await userService.RevokeAsync(login, Login, cancellationToken);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            logger.LogError(e, "Ошибка при удалении пользователя: {ErrorMessage}", e.Message);
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Восстановление пользователя
    /// </summary>
    [HttpPut("/user/{login}/restore")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RestoreAsync(string login, CancellationToken cancellationToken)
    {
        try
        {
            await userService.RestoreAsync(login, Login, cancellationToken);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            logger.LogError(e, "Ошибка при восстановлении пользователя: {ErrorMessage}", e.Message);
            return BadRequest(e.Message);
        }
    }
}