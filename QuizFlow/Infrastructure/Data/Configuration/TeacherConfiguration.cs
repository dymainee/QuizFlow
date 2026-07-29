using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizFlow.Models;

namespace QuizFlow.Infrastructure.Data.Configuration
{
    public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.Property(x => x.WorkPlace)
               .HasMaxLength(200);

            builder.Property(x => x.Specialization)
                   .HasMaxLength(200);

            
        }
    }
}
