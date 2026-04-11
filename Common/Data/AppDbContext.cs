using Microsoft.EntityFrameworkCore;
using Examination_System.Common.Models;
using ExaminationSystem.API.Common.Models;

namespace ExaminationSystem.API.Common.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Diploma> Diplomas { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Attempt> Attempts { get; set; }
        public DbSet<Answer> Answers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Quiz -> Diploma
            modelBuilder.Entity<Quiz>()
                .HasOne<Diploma>()
                .WithMany()
                .HasForeignKey(q => q.DiplomaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Question -> Quiz
            modelBuilder.Entity<Question>()
                .HasOne<Quiz>()
                .WithMany()
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            // Option -> Question
            modelBuilder.Entity<Option>()
                .HasOne<Question>()
                .WithMany()
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Attempt -> User & Quiz
            modelBuilder.Entity<Attempt>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attempt>()
                .HasOne<Quiz>()
                .WithMany()
                .HasForeignKey(a => a.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            // Answer -> Attempt, Question, Option
            modelBuilder.Entity<Answer>()
                .HasOne<Attempt>()
                .WithMany()
                .HasForeignKey(a => a.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
                .HasOne<Question>()
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Answer>()
                .HasOne<Option>()
                .WithMany()
                .HasForeignKey(a => a.OptionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
