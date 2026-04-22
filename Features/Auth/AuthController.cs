using Examination_System.Common.Wrappers;
using Examination_System.Features.Admin.Queries.GetActiveUsersToday;
using Examination_System.Features.Auth.Commands.ForgetPassword;
using Examination_System.Features.Auth.Commands.Login;
using Examination_System.Features.Auth.Commands.Registeration;
using Examination_System.Features.Auth.Commands.ResendOtp;
using Examination_System.Features.Auth.Commands.ResetPassword;
using Examination_System.Features.Auth.Commands.VerifyAccount; 
using Examination_System.Features.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserCommand request)
    {
        var result = await _mediator.Send(request);
        return result.ToActionResult();
    }

    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOtp(ResendOtpCommand command)
    {
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(VerifyAccountCommand command)
    {
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpGet("reset-password")]
    public IActionResult ResetPasswordPage([FromQuery] string token, [FromQuery] string email)
    {
        return Ok(new
        {
            Message = "Send this data to POST /api/auth/reset-password",
            Token = token,
            Email = email
        });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return result.ToActionResult();
    }


    [HttpGet("Status/Total_Users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetUsersCountQuery(), cancellationToken);

        return Ok(result);
    }

    [HttpGet("stats/active-users-today")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetActiveUsersToday(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetActiveUsersTodayQuery(), ct);
        return Ok(result);
    }
}