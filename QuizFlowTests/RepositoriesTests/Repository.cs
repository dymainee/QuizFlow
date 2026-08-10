using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
    public class Repository
    {
        // using var context (очистки памяти) после того, как метод завершит свою работу.
        //Очистка внутреннего состояния DbContext (Change Tracker)
        //Разрыв связей между тестами
        //
        private ApplicationContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationContext(options);
        }
        [Fact]
        public async Task GetByIdAsync_WhenEntityExists_ReturnsEntity()
        {
            // 1. ARRANGE
            using var context = GetInMemoryContext();
            var repo = new Repository<Quiz>(context); // Используем любую сущность, например Quiz
            var entityId = Guid.NewGuid();

            var quiz = new Quiz
            {
                Id = entityId,
                Title = "TestQuiz",
                Questions = new List<Question>()
            };

            await context.Quizzes.AddAsync(quiz);
            await context.SaveChangesAsync();

            var result = await repo.GetByIdAsync(entityId);
            var result1 = await repo.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result1); //для методов поиска по ид нужно всегда проверять на то что запись найдена или нет(NullReferenceException)

            Assert.NotNull(result);
            Assert.Equal(entityId, result.Id);
            Assert.Equal("TestQuiz", result.Title);
        }
        [Fact]
        public async Task GetAllAsync_WhenEntityExists_ReturnsListOfEntities()
        {
            using var context = GetInMemoryContext();
            var repo = new Repository<Quiz>(context);

            var quiz1 = new Quiz { Id = Guid.NewGuid(), Title = "MathQuiz" };
            var quiz2 = new Quiz { Id = Guid.NewGuid(), Title = "Advanced Math" };
            var quiz3 = new Quiz { Id = Guid.NewGuid(), Title = "History" };

            await context.Quizzes.AddRangeAsync(quiz1, quiz2, quiz3);
            await context.SaveChangesAsync();

            var result = await repo.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, q => q.Title == "MathQuiz");

        }
        [Fact]
        public async Task DeleteAsync_WhenEntityExists_RemovesEntityFromDatabase()
        {
            using var context = GetInMemoryContext();
            var repo = new Repository<Quiz>(context);
            Guid quizId = Guid.NewGuid();

            var quiz = new Quiz { Id = quizId, Title = "MathQuiz" };

            await context.Quizzes.AddAsync(quiz);
            await context.SaveChangesAsync();

            await repo.DeleteAsync(quizId);
            await context.SaveChangesAsync();

            var result = await context.Quizzes.FindAsync(quizId);
            Assert.Null(result);
            Assert.Empty(context.Quizzes);

        }

        [Fact]
        public async Task DeleteAsync_WhenEntityDoesNotExist_DoesNotThrowException()
        {
            using var context = GetInMemoryContext();
            var repo = new Repository<Quiz>(context);
            Guid quizId = Guid.NewGuid();

            await repo.DeleteAsync(quizId);

            Assert.Empty(context.Quizzes);

        }
        [Fact]
        public async Task SaveChangesAsync_WhenCalled_PersistsChangesToDatabase()
        {
            using var context = GetInMemoryContext();
            var repo = new Repository<Quiz>(context);

            var quiz = new Quiz { Id = Guid.NewGuid(), Title = "Math Quiz" };
            await context.Quizzes.AddAsync(quiz);

            await repo.SaveChangesAsync();

            var savedQuiz = await context.Quizzes.FindAsync(quiz.Id);

            Assert.NotNull(savedQuiz);
            Assert.Equal("Math Quiz", savedQuiz.Title);
        }
    }
}
