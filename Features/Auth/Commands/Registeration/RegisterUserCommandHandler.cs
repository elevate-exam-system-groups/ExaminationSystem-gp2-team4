using Examination_System.Common.Models.Identity;
using Examination_System.Common.Service.Auth;
using Examination_System.Common.Wrappers.ResultFormat;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Crypto.Generators;
using System.Data;
using System.Diagnostics.Eventing.Reader;

namespace Examination_System.Features.Auth.Commands.Registeration
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<RegisterUserResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly ILogger<RegisterUserCommandHandler> _logger;

        public RegisterUserCommandHandler(UserManager<ApplicationUser> userManager,
            IOtpService otpService, IEmailService emailService,
             ILogger<RegisterUserCommandHandler> logger)
        {
            _userManager = userManager;
            _otpService = otpService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Result<RegisterUserResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Register process started for {Email}", request.Email);

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return Result<RegisterUserResponse>.Failure("Email already exists", ErrorType.Conflict);
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
                FullName = request.FullName
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);

            if (!createResult.Succeeded)
            {
                return Result<RegisterUserResponse>.Failure(
                    "User creation failed",
                    ErrorType.UnprocessableEntity,
                    createResult.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })
                );
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Student");

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                return Result<RegisterUserResponse>.Failure("Role assignment failed", ErrorType.InternalServerError);
            }

            try
            {
                var otp = await _otpService.GenerateOtp();

                await _otpService.SaveOtpAsync(user.Email, otp);

                await _emailService.SendEmailAsync(user.Email, "Verify", $"Your OTP: {otp}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OTP or Email failed for {Email}", user.Email);

                // 💥 rollback
                await _userManager.DeleteAsync(user);

                return Result<RegisterUserResponse>.Failure(
                                              ex.Message,
                                              ErrorType.InternalServerError);

  
            }

            return Result<RegisterUserResponse>.Success(
                new RegisterUserResponse(user.Id, "User created successfully")
            );
        }
    }
}

