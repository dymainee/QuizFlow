using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
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
    public class QuizRepositoryTests
    {
        private ApplicationContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Уникальное имя гарантирует чистую БД
                .Options;

            return new ApplicationContext(options);
        }
        [Fact]
        public async Task CreateAsync_ValidQuiz_AddsQuizToDatabase() {
            var context = GetInMemoryContext();
            var repo = new QuizRepository(context);
            var quizId = Guid.NewGuid();

            var quiz = new Quiz
            {
                Id = quizId,
                Title = "Geography",
                Questions = new List<Question>()
            };

            await repo.CreateAsync(quiz);
            await context.SaveChangesAsync();

            var result = await context.Quizzes.FindAsync(quizId);
            Assert.NotNull(result);
            Assert.Single(context.Quizzes);
            Assert.Equal(quizId, result.Id);

        }
        [Fact]
        public async Task GetQuestionsAsync_WhenSessionExists_ReturnsFullHierarchy() {
            var context = GetInMemoryContext();
            var repo = new QuizRepository(context);
            var quizId = Guid.NewGuid();
            var option = new AnswerOption { Id = Guid.NewGuid(), Text = "Paris" };
            var question = new Question
            {
                Id = Guid.NewGuid(),
                Title = "Capital Question",
                Description = "Capital of France",
                AnswerOptions = new List<AnswerOption> { option }
            };
            var quiz = new Quiz
            {
                Id = quizId,
                Title = "Geography",
                Questions = new List<Question>() { question }
            };

            await context.Quizzes.AddAsync(quiz);
            await context.SaveChangesAsync();

            var result = await repo.GetQuestionsAsync(quizId);

            Assert.NotNull(result);
            Assert.Single(context.Questions);
            Assert.Equal(quizId, result.Id);
            Assert.Equal("Geography", result.Title);

            Assert.NotNull(result.Questions);
            Assert.Single(result.Questions);

            var GetOption = result.Questions.First();
            Assert.NotNull(GetOption.AnswerOptions);
            Assert.Single(GetOption.AnswerOptions);
            Assert.Equal("Paris", GetOption.AnswerOptions.First().Text);
        }
        

    }
}
