using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizFlow.Models;

namespace QuizFlow.Infrastructure.Data.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        //класс, который наследуется от User,
        //должен получить свою отдельную таблицу в БД. И свяжи эти таблицы между собой по их общему Id
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.UseTptMappingStrategy(); // TPT
            
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email)
                   .IsRequired()
                   .HasMaxLength(256);

            builder.Property(x => x.Username)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.HasIndex(x => x.Username)
                    .IsUnique();

            builder.Property(x => x.Role);
        }
    }
}
