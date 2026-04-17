using Examination_System.Common.Models;
using ExaminationSystem.API.Common.Data;
using ExaminationSystem.API.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace Examination_System.Common.Data
{
    public static class DbInitializer
    {
        public static async Task Seed(UserManager<ApplicationUser>userManager,RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            // Ensures the database and tables are created before seeding (in case they weren't yet)
            context.Database.EnsureCreated();
            
            string[] roles = { "Admin", "Student"};

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    
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
                };

                var student = new ApplicationUser
                {
                    UserName = "student@exam.com",
                    Email = "student@exam.com",
                    EmailConfirmed = true
                };

                context.Users.AddRange(admin, student);
                context.SaveChanges();

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
                context.SaveChanges();

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
                context.SaveChanges();

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
