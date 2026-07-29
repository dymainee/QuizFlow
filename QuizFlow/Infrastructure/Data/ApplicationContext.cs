using Microsoft.EntityFrameworkCore;
using QuizFlow.Infrastructure.Data.Configuration;
using QuizFlow.Models;
using QuizFlow.Models.Enums;

namespace QuizFlow.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new StudentConfiguration());
            modelBuilder.ApplyConfiguration(new TeacherConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());

        }


        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }
    }
}