using Microsoft.EntityFrameworkCore;
using QuizFlow.Data;
using QuizFlow.Infrastructure.Repositories;
using QuizFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizFlowTests.RepositoriesTests
{
    public class MenuRepositoryTest
    {
        private ApplicationContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Уникальное имя гарантирует чистую БД
                .Options;

            return new ApplicationContext(options);
        }

        [Fact]
        public async Task GetByNameAsync_WhenTitleProvided_ReturnsMatchingQuizzes()
        {
            var context = GetInMemoryContext();
            var repo = new MenuRepository(context);
            var quiz1 = new Quiz { Id = Guid.NewGuid(), Title = "Math Quiz" };
            var quiz2 = new Quiz { Id = Guid.NewGuid(), Title = "Advanced Math" };
            var quiz3 = new Quiz { Id = Guid.NewGuid(), Title = "History" };

            await context.Quizzes.AddRangeAsync(quiz1, quiz2, quiz3);
            await context.SaveChangesAsync();

            var result = await repo.GetByNameAsync("Math");

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, q => Assert.Contains("Math", q.Title)); //// Каждый квиз содержит "Math"

        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task GetByNameAsync_WhenTitleIsNullOrEmpty_ReturnsAllQuizzes(string invalidTitle)
        {
            using var context = GetInMemoryContext();
            var repo = new MenuRepository(context);

            var quiz1 = new Quiz { Id = Guid.NewGuid(), Title = "Math" };
            var quiz2 = new Quiz { Id = Guid.NewGuid(), Title = "History" };

            await context.Quizzes.AddRangeAsync(quiz1, quiz2);
            await context.SaveChangesAsync();
            var result = await repo.GetByNameAsync(invalidTitle);
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
    }
}
