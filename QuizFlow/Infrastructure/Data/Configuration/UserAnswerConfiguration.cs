using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizFlow.Models;

namespace QuizFlow.Infrastructure.Data.Configuration
{
    public class UserAnswerConfiguration : IEntityTypeConfiguration<UserAnswer>
    {
        public void Configure(EntityTypeBuilder<UserAnswer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.QuizSession)
                   .WithMany(x => x.UserAnswers)
                   .HasForeignKey(x => x.QuizSessionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
