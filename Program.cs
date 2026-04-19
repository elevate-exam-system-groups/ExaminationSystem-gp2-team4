using Examination_System.Common.Data;
using Examination_System.Common.Extensions.DependencyInjection;
using Examination_System.Common.Models;
using Examination_System.Common.Models.Identity;
using ExaminationSystem.API.Common.Data;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Load local config
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// Controllers + OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// DI
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var dbContext = services.GetRequiredService<AppDbContext>();

        await DbInitializer.Seed(userManager, roleManager, dbContext);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
}

// Pipeline
if (app.Environment.IsDevelopment())
{

    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Examination System API";
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/scalar"));

app.Run();

