using Examination_System.Common.Data;
using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Scalar.AspNetCore;
using ExaminationSystem.API.Common.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Adding my own appsettings.Local.json just while testing 
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddMemoryCache();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
   var services = scope.ServiceProvider;
   try
   {
       var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
       var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
       
       await DbInitializer.Seed(userManager, roleManager, services.GetRequiredService<AppDbContext>());
   }
   catch (Exception e)
   {
       Console.WriteLine(e);
       throw;
   }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
