using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizFlow.Models;

namespace QuizFlow.Infrastructure.Data.Configuration
{
    public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
    {
        public void Configure(EntityTypeBuilder<Quiz> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status);

            builder
                .HasOne(x => x.Teacher)
                .WithMany(x => x.Quizzes)
                .HasForeignKey(x => x.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
