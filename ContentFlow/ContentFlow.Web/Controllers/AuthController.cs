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
        
        var  result = await _mediator.Send(userCommand);
        _logger.LogInformation("Register completed result = {Result}. With error message = {errorMessage}", result.Success, result.Errors);
        
        return Ok(result); // Todo: RegisterUser Добавить обработку ошибок
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
        _logger.LogInformation("Metadata compilated");
        
        var loginCommand = new LoginCommand(loginRequest.Email, loginRequest.Password, metadata);
        
        var result = await _mediator.Send(loginCommand);
        _logger.LogInformation("Login method completed result login = {Result}. With errors message = {errorMessage}", result.Success, result.Errors);
        
        return Ok(result); // Todo: Login Добавить обработку ошибок. 
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand confirmEmailCommand)
    {
        _logger.LogInformation("Start confirm email");
        
        var result = await _mediator.Send(confirmEmailCommand);
        _logger.LogInformation("Email confirmation completed result = {Result}. With errors message = {errorMessage}", result.Success, result.Errors);
        
        return Ok(result); // Todo: ConfirmEmail Добавить обработку ошибок.
    }

    [HttpPost("resend-confirmation")]
    public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationCommand resendConfirmationCommand)
    {
        _logger.LogInformation("Start resend confirmation to email {email}", resendConfirmationCommand.Email);
        
        var result = await _mediator.Send(resendConfirmationCommand);
        _logger.LogInformation("Result resend confirmation to emailAddress {emailAddress}. Result {result}. With errorMessage {errorMessage}",
            resendConfirmationCommand.Email, result.Success, result.Errors);
        
        return Ok(result); // Todo: ResendConfirmation Добавить обработку ошибок.
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
        
        return Ok(result);
    }
    
    private string GetIpAddress()
    {
        if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            return HttpContext.Request.Headers["X-Forwarded-For"];
    
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}