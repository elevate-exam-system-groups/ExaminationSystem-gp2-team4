using Microsoft.EntityFrameworkCore;
using Examination_System.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ExaminationSystem.API.Common.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Diploma> Diplomas { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Attempt> Attempts { get; set; }
        public DbSet<Answer> Answers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Diploma)
                .WithMany(d => d.Quizzes)
                .HasForeignKey(q => q.DiplomaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz)
<<<<<<< HEAD
                .WithMany(qz =>qz.Questions)
=======
                .WithMany(quiz => quiz.Questions)
>>>>>>> Create-Manage-Quizzes
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Option>()
<<<<<<< HEAD
                .HasOne(q=>q.Question)
                .WithMany(o=>o.Options)
=======
                .HasOne(o => o.Question)
                .WithMany(q => q.Options)
>>>>>>> Create-Manage-Quizzes
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attempt>()
<<<<<<< HEAD
                .HasOne(u=>u.User)
                .WithMany(u=>u.Attempts)
=======
                .HasOne(a => a.User)
                .WithMany()
>>>>>>> Create-Manage-Quizzes
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attempt>()
<<<<<<< HEAD
                .HasOne(q=>q.Quiz)
                .WithMany(q=>q.Attempts)
=======
                .HasOne(a => a.Quiz)
                .WithMany(q => q.Attempts)
>>>>>>> Create-Manage-Quizzes
                .HasForeignKey(a => a.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
<<<<<<< HEAD
                .HasOne(a=>a.Attempt)
                .WithMany(at=>at.Answers)
=======
                .HasOne(a => a.Attempt)
                .WithMany()
>>>>>>> Create-Manage-Quizzes
                .HasForeignKey(a => a.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
<<<<<<< HEAD
                .HasOne(q=>q.Question)
=======
                .HasOne(a => a.Question)
>>>>>>> Create-Manage-Quizzes
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Answer>()
<<<<<<< HEAD
                .HasOne(o=>o.Option)
=======
                .HasOne(a => a.Option)
>>>>>>> Create-Manage-Quizzes
                .WithMany()
                .HasForeignKey(a => a.OptionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
