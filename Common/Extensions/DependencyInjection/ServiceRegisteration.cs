using Examination_System.Common.Models.Identity;
using Examination_System.Common.Service.Auth;
using Examination_System.Features.Auth.Commands.Registeration;
using FluentAssertions.Common;

namespace Examination_System.Common.Extensions.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Auth Services
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IEmailService, EmailService>();


            // MediatR
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommandHandler).Assembly));


            return services;
        }
    }
}
