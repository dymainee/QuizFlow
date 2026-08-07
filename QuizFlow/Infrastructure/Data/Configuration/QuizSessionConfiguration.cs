using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizFlow.Models;

namespace QuizFlow.Infrastructure.Data.Configuration
{
    public class QuizSessionConfiguration : IEntityTypeConfiguration<QuizSession>
    {
        public void Configure(EntityTypeBuilder<QuizSession> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Quiz)
                .WithMany(x => x.QuizSessions)
                .HasForeignKey(x => x.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Student)           
               .WithMany(x => x.QuizSessions)                        
               .HasForeignKey(x => x.UserId)    
               .OnDelete(DeleteBehavior.NoAction);

        }

    }
}
