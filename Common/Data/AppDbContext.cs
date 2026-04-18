using Microsoft.EntityFrameworkCore;
using Examination_System.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Examination_System.Common.Models.Identity;

namespace ExaminationSystem.API.Common.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>

    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Diploma> Diplomas { get; set; }
        public DbSet<EmailSettings> EmailSettings { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<OtpCode> OtpCodes { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Attempt> Attempts { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().ToTable("Users");

            modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles");

            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");

            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");

            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");

            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");

            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");




            // Student 
            modelBuilder.Entity<ApplicationUser>()
                          .HasOne(u => u.Student)
                          .WithOne(s => s.User)
                          .HasForeignKey<Student>(s => s.Id);


            //Pass 
            modelBuilder.Entity<PasswordResetToken>()
                        .HasOne(t => t.User)
                        .WithMany(u => u.PasswordResetTokens)
                        .HasForeignKey(t => t.UserId)
                        .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Diploma)
                .WithMany(d => d.Quizzes)
                .HasForeignKey(q => q.DiplomaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(qz =>qz.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Option>()
                .HasOne(q=>q.Question)
                .WithMany(o=>o.Options)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

modelBuilder.Entity<Attempt>()
                .HasOne(u => u.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attempt>()
                .HasOne(a => a.Quiz)
                .WithMany(q => q.Attempts)
                .HasForeignKey(a => a.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Option)
                .WithMany()
                .HasForeignKey(a => a.OptionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
