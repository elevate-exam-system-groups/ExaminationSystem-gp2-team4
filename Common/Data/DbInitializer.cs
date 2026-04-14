using Examination_System.Common.Models;
using Examination_System.Common.Models.Identity;
using ExaminationSystem.API.Common.Data;
using ExaminationSystem.API.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Common.Data
{
   
      
       public static class DbInitializer
        {
            public static async Task SeedAsync(
                AppDbContext context,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole<Guid>> roleManager)
            {
                await context.Database.MigrateAsync();

                // 1. Seed Roles
                string[] roles = { "Admin", "Student" };

                foreach (var role in roles)
                {
                    var roleExists = await roleManager.RoleExistsAsync(role);
                    if (!roleExists)
                    {
                        await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                    }
                }

                // 2. Seed Admin User
                var adminEmail = "admin@exam.com";

                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        Id = Guid.NewGuid(),
                        UserName = adminEmail,
                        Email = adminEmail,
                        FullName = "System Admin",
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(adminUser, "Admin@123");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }

                // 3. Seed Student User
                var studentEmail = "student@exam.com";

                var studentUser = await userManager.FindByEmailAsync(studentEmail);

                if (studentUser == null)
                {
                    studentUser = new ApplicationUser
                    {
                        Id = Guid.NewGuid(),
                        UserName = studentEmail,
                        Email = studentEmail,
                        FullName = "John Doe",
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(studentUser, "Student@123");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(studentUser, "Student");
                    }
                }

                // 4. Seed Domain Data (NOT Identity)
                if (!await context.Diplomas.AnyAsync())
                {
                    var diploma = new Diploma
                    {
                        Id = Guid.NewGuid(),
                        Title = "Software Engineering Fundamentals",
                        Description = "Basic introduction to software engineering concepts.",
                        IsActive = true,
                        ImageUrl = "default.png",
                        CreatedAt = DateTime.UtcNow
                    };

                    await context.Diplomas.AddAsync(diploma);
                    await context.SaveChangesAsync();

                    var quiz = new Quiz
                    {
                        Id = Guid.NewGuid(),
                        DiplomaId = diploma.Id,
                        Title = "C# Basics",
                        DurationMinutes = 30,
                        Status = "Active",
                        PassScore = 50,
                        QuestionsCount = 2,
                        CreatedAt = DateTime.UtcNow
                    };

                    await context.Quizzes.AddAsync(quiz);
                    await context.SaveChangesAsync();

                    var q1 = new Question
                    {
                        Id = Guid.NewGuid(),
                        QuizId = quiz.Id,
                        Body = "What does OOP stand for?",
                        Type = "MultipleChoice",
                        OrderIndex = 1,
                        CreatedAt = DateTime.UtcNow
                    };

                    var q2 = new Question
                    {
                        Id = Guid.NewGuid(),
                        QuizId = quiz.Id,
                        Body = "Which keyword is used for inheritance in C#?",
                        Type = "MultipleChoice",
                        OrderIndex = 2,
                        CreatedAt = DateTime.UtcNow
                    };

                    await context.Questions.AddRangeAsync(q1, q2);
                    await context.SaveChangesAsync();

                    var options = new List<Option>
            {
                new Option
                {
                    Id = Guid.NewGuid(),
                    QuestionId = q1.Id,
                    Body = "Object-Oriented Programming",
                    IsCorrect = true,
                    OrderIndex = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new Option
                {
                    Id = Guid.NewGuid(),
                    QuestionId = q1.Id,
                    Body = "Object-Oriented Platform",
                    IsCorrect = false,
                    OrderIndex = 2,
                    CreatedAt = DateTime.UtcNow
                },
                new Option
                {
                    Id = Guid.NewGuid(),
                    QuestionId = q2.Id,
                    Body = "class Child : Parent",
                    IsCorrect = true,
                    OrderIndex = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new Option
                {
                    Id = Guid.NewGuid(),
                    QuestionId = q2.Id,
                    Body = "extends",
                    IsCorrect = false,
                    OrderIndex = 2,
                    CreatedAt = DateTime.UtcNow
                }
            };

                    await context.Options.AddRangeAsync(options);
                    await context.SaveChangesAsync();
                }
            }
        }
}

