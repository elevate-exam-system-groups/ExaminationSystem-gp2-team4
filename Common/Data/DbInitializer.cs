using Examination_System.Common.Models;
using ExaminationSystem.API.Common.Data;
using ExaminationSystem.API.Common.Models;

namespace Examination_System.Common.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            // Ensures the database and tables are created before seeding (in case they weren't yet)
            context.Database.EnsureCreated();

            if (!context.Users.Any())
            {
                var admin = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "System Admin",
                    Email = "admin@exam.com",
                    PasswordHash = "hashedpassword_mock",
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                };

                var student = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "John Doe",
                    Email = "student@exam.com",
                    PasswordHash = "hashedpassword_mock",
                    Role = "Student",
                    CreatedAt = DateTime.UtcNow
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
                    status = "Active",
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
