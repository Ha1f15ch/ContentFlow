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

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterCommand userCommand)
    {
        var  result = await _mediator.Send(userCommand);
        return Ok(result);
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
        
        var loginCommand = new LoginCommand(loginRequest.Email, loginRequest.Password, metadata);
        var result = await _mediator.Send(loginCommand);
        
        return Ok(result);
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand confirmEmailCommand)
    {
        var result = await _mediator.Send(confirmEmailCommand);
        return Ok(result);
    }

    [HttpPost("resend-confirmation")]
    public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationCommand resendConfirmationCommand)
    {
        var result = await _mediator.Send(resendConfirmationCommand);
        return Ok(result);
    }
    
    private string GetIpAddress()
    {
        if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            return HttpContext.Request.Headers["X-Forwarded-For"];
    
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}