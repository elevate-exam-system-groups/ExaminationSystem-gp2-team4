using Examination_System.Common.Models;
using Examination_System.Common.Models.Identity;
using Examination_System.Common.Repositories;
using Examination_System.Common.Repositories.Implementaition;
using Examination_System.Common.Service.Auth;
using Examination_System.Features.Auth.Commands.Registeration;
using Examination_System.Infrastructure.Repositories;
using Examination_System.Repositories;
using ExaminationSystem.API.Common.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Examination_System.Common.Extensions.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ================= DB =================
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // ================= Identity =================
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // ================= Cache =================
            services.AddMemoryCache();

            // ================= Auth Services =================
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPasswordResetTokenRepository,PasswordResetTokenRepository>();

            // ================= JWT And Email=================
            services.Configure<JwtSettings>(
                configuration.GetSection("JwtSettings"));
            services.Configure<EmailSettings>(
                 configuration.GetSection("EmailSettings"));

            services.AddSingleton(sp =>
                sp.GetRequiredService<IOptions<JwtSettings>>().Value);

            // ================= Repositories =================
            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IStudentRepository, StudentRepository>();

            // ================= Authorization =================
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ManageExams",
                    policy => policy.RequireRole("Teacher"));

                options.AddPolicy("TakeExam",
                    policy => policy.RequireRole("Student"));

                options.AddPolicy("StudentOnly",
                    policy => policy.RequireRole("Student"));
            });

            // ================= MediatR =================
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommandHandler).Assembly));
            
            // 
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = configuration["JwtSettings:Issuer"],
                        ValidAudience = configuration["JwtSettings:Audience"],

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"])),

                        RoleClaimType = System.Security.Claims.ClaimTypes.Role
                    };
                });

            return services;
        }
    }
}