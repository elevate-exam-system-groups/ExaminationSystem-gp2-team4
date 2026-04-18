using Examination_System.Common.Models;
using ExaminationSystem.API.Common.Data;
<<<<<<< HEAD
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
=======
using ExaminationSystem.API.Common.Models;
using Microsoft.AspNetCore.Identity;
>>>>>>> origin/Test

namespace Examination_System.Common.Data
{
    public static class DbInitializer
    {
<<<<<<< HEAD
        public static async Task Seed(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            AppDbContext context)
        {

            await context.Database.MigrateAsync();
            // ================= ROLES =================
            string[] roles = { "Admin", "Student" };
=======
        public static async Task Seed(UserManager<ApplicationUser>userManager,RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            // Ensures the database and tables are created before seeding (in case they weren't yet)
            context.Database.EnsureCreated();
            
            string[] roles = { "Admin", "Student"};
>>>>>>> origin/Test

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
<<<<<<< HEAD
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            // ================= USERS =================
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
=======
                    
                        await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            
            if (!context.Users.Any())
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@exam.com",
                    Email = "admin@exam.com",
                    EmailConfirmed = true
>>>>>>> origin/Test
                };

                var student = new ApplicationUser
                {
<<<<<<< HEAD
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
                    await userManager.AddToRoleAsync(admin, "Admin");

                if (studentResult.Succeeded)
                    await userManager.AddToRoleAsync(student, "Student");
            }

            // ================= DIPLOMA =================
            if (!context.Diplomas.Any())
            {
=======
                    UserName = "student@exam.com",
                    Email = "student@exam.com",
                    EmailConfirmed = true
                };

                context.Users.AddRange(admin, student);
                context.SaveChanges();

>>>>>>> origin/Test
                var diploma = new Diploma
                {
                    Id = Guid.NewGuid(),
                    Title = "Software Engineering Fundamentals",
                    Description = "Basic introduction to software engineering concepts.",
                    IsActive = true,
                    ImageUrl = "default.png",
                    CreatedAt = DateTime.UtcNow
                };

                context.Diplomas.Add(diploma);
<<<<<<< HEAD
                await context.SaveChangesAsync();

                // ================= QUIZ =================
=======
                context.SaveChanges();

>>>>>>> origin/Test
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

                context.Quizzes.Add(quiz);
<<<<<<< HEAD
                await context.SaveChangesAsync();

                // ================= QUESTIONS =================
=======
                context.SaveChanges();

>>>>>>> origin/Test
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

                context.Questions.AddRange(q1, q2);
<<<<<<< HEAD
                await context.SaveChangesAsync();

                // ================= OPTIONS =================
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
                        Body = ":",
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

                context.Options.AddRange(options);
                await context.SaveChangesAsync();
            }
        }
    }
}
=======
                context.SaveChanges();

                var op1 = new Option { Id = Guid.NewGuid(), QuestionId = q1.Id, Body = "Object-Oriented Programming", IsCorrect = true, OrderIndex = 1, CreatedAt = DateTime.UtcNow };
                var op2 = new Option { Id = Guid.NewGuid(), QuestionId = q1.Id, Body = "Object-Oriented Platform", IsCorrect = false, OrderIndex = 2, CreatedAt = DateTime.UtcNow };

                var op3 = new Option { Id = Guid.NewGuid(), QuestionId = q2.Id, Body = ":", IsCorrect = true, OrderIndex = 1, CreatedAt = DateTime.UtcNow };
                var op4 = new Option { Id = Guid.NewGuid(), QuestionId = q2.Id, Body = "extends", IsCorrect = false, OrderIndex = 2, CreatedAt = DateTime.UtcNow };

                context.Options.AddRange(op1, op2, op3, op4);
                context.SaveChanges();
            }
        }
    }
}
>>>>>>> origin/Test
