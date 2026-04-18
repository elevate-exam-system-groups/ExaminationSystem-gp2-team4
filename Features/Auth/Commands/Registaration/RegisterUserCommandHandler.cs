using Examination_System.Common.Models;
using Examination_System.Common.Models.Identity;
using Examination_System.Common.Repositories;
using Examination_System.Common.Service.Auth;
using Examination_System.Common.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Examination_System.Features.Auth.Commands.Registeration
{
    public class RegisterUserCommandHandler
        : IRequestHandler<RegisterUserCommand, ApiResponse<RegisterUserResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly IStudentRepository _studentRepository;

        public RegisterUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            IOtpService otpService,
            IEmailService emailService,
            IStudentRepository studentRepository)
        {
            _userManager = userManager;
            _otpService = otpService;
            _emailService = emailService;
            _studentRepository = studentRepository;
        }

        public async Task<ApiResponse<RegisterUserResponse>> Handle(
            RegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return ApiResponse<RegisterUserResponse>.Failure(ErrorCode.EmailAlreadyExists);
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
                FullName = request.FullName,
                EmailConfirmed = false
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);

            if (!createResult.Succeeded)
            {
                return ApiResponse<RegisterUserResponse>.Failure(ErrorCode.PasswordTooWeak);
            }

            try
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "Student");

                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return ApiResponse<RegisterUserResponse>.Failure(ErrorCode.InternalServerError);
                }

                var student = new Student
                {
                    Id = user.Id,
                    Status = AccountStatus.Pending
                };

                await _studentRepository.AddAsync(student, cancellationToken);

                var otp = await _otpService.GenerateOtp();
                await _otpService.SaveOtpAsync(user.Email, otp);

                await _emailService.SendEmailAsync(
                    user.Email,
                    "Verify Account",
                    $"Your OTP is: {otp}"
                );
            }
            catch
            {
                await _userManager.DeleteAsync(user);
                return ApiResponse<RegisterUserResponse>.Failure(ErrorCode.InternalServerError);
            }

            return ApiResponse<RegisterUserResponse>.Success(
                new RegisterUserResponse(user.Id, "User created successfully")
            );
        }
    }
}