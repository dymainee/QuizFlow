using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Moq;
using QuizFlow.Application.Services;
using QuizFlow.Data;
using QuizFlow.Infrastructure.Interfaces;
using QuizFlow.Infrastructure.Repositories;
using QuizFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QuizFlowTests.RepositoriesTests
{//Мок (Mock) — это программный объект-заглушка, который имитирует поведение реального класса или интерфейса в юнит-тестах.
    public class QuizSessionRepositoryTests
    {
        private ApplicationContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Уникальное имя гарантирует чистую БД
                .Options;

            return new ApplicationContext(options);
        }
        [Fact]
        public async Task GetSessionsByTeacherAsync_ReturnsOnlySessionsForSpecificTeacher() // 
        {
            using var context = GetInMemoryContext();
            var teacher1Id = Guid.NewGuid();
            var teacher2Id = Guid.NewGuid();

            var quiz1 = new Quiz { Id = Guid.NewGuid(), Title = "Math", TeacherId = teacher1Id };
            var quiz2 = new Quiz { Id = Guid.NewGuid(), Title = "History", TeacherId = teacher2Id };



            var student = new Student { Id = Guid.NewGuid(), Name = "John" };

            context.Quizzes.AddRange(quiz1, quiz2);
            context.Students.Add(student);

            context.quizSessions.AddRange(
                new QuizSession { Id = Guid.NewGuid(), Quiz = quiz1, Student = student },
                 new QuizSession { Id = Guid.NewGuid(), Quiz = quiz2, Student = student }
                );

            await context.SaveChangesAsync();

            var repo = new QuizSessionRepository(context);

            var result = await repo.GetSessionsByTeacherAsync(teacher1Id);
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(teacher1Id, result.First().Quiz.TeacherId);
        }

        [Fact]
        public async Task GetSessionWithDetailsAsync_WhenSessionExists_ReturnsFullHierarchy()
        {
            using var context = GetInMemoryContext();
            Guid sessionid = Guid.NewGuid();
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
                Id = Guid.NewGuid(),
                Title = "Georpahy",
                Questions = new List<Question> { question }
            };

            var userAnswer = new UserAnswer { Id = Guid.NewGuid(), SelectedOptionId = option.Id };

            var session = new QuizSession
            {
                Id = sessionid,
                Quiz = quiz,
                UserAnswers = new List<UserAnswer> { userAnswer }
            };
            context.quizSessions.Add(session);
            await context.SaveChangesAsync();
            var repo = new QuizSessionRepository(context);
            var result = await repo.GetSessionWithDetailsAsync(sessionid);
            Assert.NotNull(result);
            Assert.Equal(sessionid, result.Id);

            Assert.NotNull(result.Quiz);
            Assert.Equal("Georpahy", result.Quiz.Title);

            Assert.NotNull(result.Quiz.Questions);
            Assert.Single(result.Quiz.Questions);

            var firstQuestion = result.Quiz.Questions.First();
            Assert.NotNull(firstQuestion.AnswerOptions);
            Assert.Single(firstQuestion.AnswerOptions);
            Assert.Equal("Paris", firstQuestion.AnswerOptions.First().Text);

            Assert.NotNull(result.UserAnswers);
            Assert.Single(result.UserAnswers);
        }

        [Fact]
        public async Task AddAsync_ValidSession_AddsSessionToDatabase()
        {
            var context = GetInMemoryContext();
            var repo = new QuizSessionRepository(context);

            var sessionid = Guid.NewGuid();
            var quiz = new Quiz
            {
                Id = Guid.NewGuid(),
                Title = "Georpahy",
                Questions = new List<Question>()
            };
            var session = new QuizSession
            {
                Id = sessionid,
                Quiz = quiz,
                UserAnswers = new List<UserAnswer>()
            };

            await repo.AddAsync(session);
            await context.SaveChangesAsync();


            var sessionInDb = await context.quizSessions.FindAsync(sessionid);

            Assert.NotNull(sessionInDb);
            Assert.Single(context.quizSessions);
            Assert.Equal(sessionid, sessionInDb.Id);
        }

    }
}
