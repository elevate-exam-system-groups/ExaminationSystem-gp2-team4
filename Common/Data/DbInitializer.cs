using Examination_System.Common.Models;
using Examination_System.Common.Models.Identity;
using ExaminationSystem.API.Common.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Common.Data
{
    public static class DbInitializer
    {
        public static async Task Seed(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            AppDbContext context)
        {
            await context.Database.MigrateAsync();

            string[] roles = { "Admin", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            if (!userManager.Users.Any())
            {
                var admin = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "admin@exam.com",
                    Email = "admin@exam.com",
                    EmailConfirmed = true,
                    FullName = "System Admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var student = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "student@exam.com",
                    Email = "student@exam.com",
                    EmailConfirmed = true,
                    FullName = "Test Student",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var adminResult = await userManager.CreateAsync(admin, "Admin@123");
                var studentResult = await userManager.CreateAsync(student, "Student@123");

                if (adminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }

                if (studentResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(student, "Student");

                    if (!await context.Students.AnyAsync(s => s.Id == student.Id))
                    {
                        context.Students.Add(new Student
                        {
                            Id = student.Id,
                            Status = AccountStatus.Active
                        });
                    }

                    await context.SaveChangesAsync();
                }
            }

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

                var quiz = new Quiz
                {
                    Id = Guid.NewGuid(),
                    DiplomaId = diploma.Id,
                    Title = "C# Basics",
                    DurationMinutes = 30,
                    Status = "Active",
                    PassScore = 50,
                    QuestionsCount = 2,
                    MaxAttempts = 3,
                    Instructions = "Read each question carefully before submitting.",
                    CreatedAt = DateTime.UtcNow
                };

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
                    Body = "Which keyword is used to inherit a class in C#?",
                    Type = "MultipleChoice",
                    OrderIndex = 2,
                    CreatedAt = DateTime.UtcNow
                };

                var options = new List<Option>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        QuestionId = q1.Id,
                        Body = "Object-Oriented Programming",
                        IsCorrect = true,
                        OrderIndex = 1,
                        CreatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        QuestionId = q1.Id,
                        Body = "Object-Oriented Platform",
                        IsCorrect = false,
                        OrderIndex = 2,
                        CreatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        QuestionId = q2.Id,
                        Body = ":",
                        IsCorrect = true,
                        OrderIndex = 1,
                        CreatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        QuestionId = q2.Id,
                        Body = "extends",
                        IsCorrect = false,
                        OrderIndex = 2,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                context.Diplomas.Add(diploma);
                context.Quizzes.Add(quiz);
                context.Questions.AddRange(q1, q2);
                context.Options.AddRange(options);

                await context.SaveChangesAsync();
            }
        }
    }
}
