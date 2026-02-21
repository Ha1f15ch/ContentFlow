using System.Security.Claims;
using ContentFlow.Application.Common;
using ContentFlow.Application.Functions.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;

namespace ContentFlow.Web.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IMediator mediator,
        ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterCommand userCommand)
    {
        _logger.LogInformation("Start register");

        try
        {
            var result = await _mediator.Send(userCommand);
            _logger.LogInformation("Register completed. Success: {Success}. Errors: {@Errors}",
                result.Success, result.Errors);

            if (!result.Success)
            {
                return BadRequest(new { message = "Registration failed", errors = result.Errors });
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Operation process was not successful during registration.");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Internal Server Error during registration.");
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var metadata = new ClientMetadata(
            IpAddress: GetIpAddress(),
            UserAgent: HttpContext.Request.Headers.UserAgent,
            DeviceId: Request.Headers["DeviceId"],
            Location: null
        );
        _logger.LogInformation("Metadata compiled");

        var loginCommand = new LoginCommand(loginRequest.Email, loginRequest.Password, metadata);

        try
        {
            var result = await _mediator.Send(loginCommand);
            _logger.LogInformation("Login completed. Success: {Success}. Errors: {@Errors}",
                result.Success, result.Errors);

            if (!result.Success)
            {
                return BadRequest(new { message = "Login failed", errors = result.Errors });
            }
            
            Response.Cookies.Append("refresh_token", result.RefreshToken!, new CookieOptions
            {
                HttpOnly = true,
#if DEBUG
                Secure = false,
#else
            Secure = true,
#endif
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Path = "/api/auth"
            });

            return Ok(new { token = result.Token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login.");
            return StatusCode(500, new { message = "An unexpected error occurred during login." });
        }
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand confirmEmailCommand)
    {
        _logger.LogInformation("Start confirm email");
    
        var result = await _mediator.Send(confirmEmailCommand);
        _logger.LogInformation("Email confirmation completed. Success = {Success}, Errors = {Errors}", 
            result.Success, result.Errors);

        if (!result.Success)
        {
            return BadRequest(new { Success = false, Errors = result.Errors });
        }

        return Ok(new { Success = true });
    }

    [HttpPost("resend-confirmation")]
    public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationCommand resendConfirmationCommand)
    {
        _logger.LogInformation("Start resend confirmation to email {Email}", resendConfirmationCommand.Email);
    
        var result = await _mediator.Send(resendConfirmationCommand);
        _logger.LogInformation("Resend confirmation result for {Email}. Success = {Success}, Errors = {Errors}",
            resendConfirmationCommand.Email, result.Success, result.Errors);

        if (!result.Success)
        {
            return BadRequest(new { Success = false, Errors = result.Errors });
        }

        return Ok(new { Success = true });
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        _logger.LogInformation("Start logout");
        
        var userIdClaims = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaims, out var userId))
            return Unauthorized();
        
        _logger.LogInformation("User id = {userLogout} try logout", userId);
        var command = new LogoutCommand(UserId: userId);

        var result = await _mediator.Send(command);
        _logger.LogInformation("End logout. With result = {resultLogout}", result);

        if (result)
        {
            Response.Cookies.Delete("refresh_token", new CookieOptions { Path = "/api/auth" });
            return Ok(result);
        }
        else
        {
            return BadRequest(new { message = "Logout failed" });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refresh = Request.Cookies["refresh_token"];
        if (string.IsNullOrWhiteSpace(refresh))
            return Unauthorized(new { message = "Refresh token missing" });

        var metadata = new ClientMetadata(
            IpAddress: GetIpAddress(),
            UserAgent: HttpContext.Request.Headers.UserAgent,
            DeviceId: Request.Headers["DeviceId"],
            Location: null
        );

        var result = await _mediator.Send(new RefreshCommand(refresh, metadata));

        if (!result.Success)
            return Unauthorized(new { message = "Refresh failed", errors = result.Errors });
        
        Response.Cookies.Append("refresh_token", result.RefreshToken!, new CookieOptions
        {
            HttpOnly = true,
#if DEBUG
            Secure = false,
#else
            Secure = true,
#endif
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/api/auth"
        });
        
        return Ok(new { token = result.Token });
    }
    
    private string GetIpAddress()
    {
        if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            return HttpContext.Request.Headers["X-Forwarded-For"];
    
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}