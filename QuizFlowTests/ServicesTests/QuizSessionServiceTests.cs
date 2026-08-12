using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Moq;
using QuizFlow.Application.Services;
using QuizFlow.DTO;
using QuizFlow.Infrastructure.Interfaces;
using QuizFlow.Infrastructure.Repositories;
using QuizFlow.Models;
using QuizFlow.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizFlowTests.ServicesTests
{
    public class QuizSessionServiceTests
    {
        private readonly Mock<IQuizSessionRepository> _quizSessionRepositoryMock;
        private readonly QuizSessionService _quizSessionService;

        public QuizSessionServiceTests()
        {
            _quizSessionRepositoryMock = new Mock<IQuizSessionRepository>();
            _quizSessionService = new QuizSessionService(_quizSessionRepositoryMock.Object);//то готовый экземпляр интерфейса
        }
        [Fact]
        public async Task StartSessionAsync_ValidData_CreatesSessionAndReturnsId()
        {
            Guid userId = Guid.NewGuid();
            Guid quizId = Guid.NewGuid();
            string groupName = "Math";

            Guid resultId = await _quizSessionService.StartSessionAsync(userId, quizId, groupName);

            Assert.NotEqual(Guid.Empty, resultId);

            _quizSessionRepositoryMock.Verify(x => x.AddAsync(It.Is<QuizSession>(s =>
                    s.UserId == userId &&
                    s.QuizId == quizId &&
                    s.GroupName == groupName &&
                    s.Score == 0)), Times.Once);

            _quizSessionRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetQuestionsAsync_ValidSessionAndQuestionNumber_ReturnsQuestionPageDto()
        {
            var sessionId = Guid.NewGuid();
            var questionId = Guid.NewGuid();
            var optionId = Guid.NewGuid();

            var question = new Question
            {
                Id = questionId,
                Title = "Question 1",
                Description = "Description 1",
                ImagePath = "image.jpg",
                AnswerOptions = new List<AnswerOption> { new AnswerOption { Id = optionId, Text = "Option A" } }

            };
            var session = new QuizSession
            {
                Id = sessionId,
                Quiz = new Quiz
                {
                    Questions = new List<Question> { question }
                },
                UserAnswers = new List<UserAnswer>()
            };

            _quizSessionRepositoryMock
                .Setup(r => r.GetSessionWithDetailsAsync(sessionId))
                .ReturnsAsync(session);

            var result = await _quizSessionService.GetQuestionsAsync(sessionId, 1);

            Assert.NotNull(result);
            Assert.Equal(sessionId, result.SessionId);
            Assert.Equal(questionId, result.QuestionId);
            Assert.Equal("Question 1", result.QuestionTitle);
            Assert.Equal(1, result.CurrentQuestionNumber);
            Assert.Equal(1, result.TotalQuestions);
            Assert.Null(result.SelectedOptionId);
        }
        [Fact]
        public async Task GetQuestionsAsync_SessionNotFound_ReturnsNull()
        {
            var sessionId = Guid.NewGuid();

            _quizSessionRepositoryMock
                .Setup(r => r.GetSessionWithDetailsAsync(sessionId))
                .ReturnsAsync(() => null);
            var result = await _quizSessionService.GetQuestionsAsync(sessionId, 1);

            Assert.Null(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(4)]
        public async Task GetQuestionsAsync_InvalidQuestionNumber_ReturnsNull(int invalidQuestionNumber)
        {
            var sessionId = Guid.NewGuid();

            var session = new QuizSession
            {
                Id = sessionId,
                Quiz = new Quiz
                {
                    Questions = new List<Question> { new Question { Id = Guid.NewGuid() }, new Question { Id = Guid.NewGuid() } }
                },
                UserAnswers = new List<UserAnswer>()

            };

            _quizSessionRepositoryMock.Setup(x => x.GetSessionWithDetailsAsync(sessionId)).ReturnsAsync(session);

            var result = await _quizSessionService.GetQuestionsAsync(sessionId, invalidQuestionNumber);
            Assert.Null(result);
        }

        [Fact]
        public async Task SubmitAnswerAsync_NewAnswer_CreatesUserAnswerAndReturnsTrue()
        {
            var sessionId = Guid.NewGuid();
            var questionId = Guid.NewGuid();
            var selectedOptionId = Guid.NewGuid();

            var session = new QuizSession
            {
                Id = sessionId,
                UserAnswers = new List<UserAnswer>()
            };

            _quizSessionRepositoryMock.Setup(x => x.GetSessionWithDetailsAsync(sessionId)).ReturnsAsync(session);

            var result = await _quizSessionService.SubmitAnswerAsync(sessionId, questionId, selectedOptionId);

            Assert.True(result);
            Assert.Single(session.UserAnswers);
            var createdAnswer = session.UserAnswers.First();

            Assert.Equal(sessionId, createdAnswer.QuizSessionId);
            Assert.Equal(questionId, createdAnswer.QuestionId);
            Assert.Equal(selectedOptionId, createdAnswer.SelectedOptionId);

        }

        [Fact]
        public async Task SubmitAnswerAsync_ExistingAnswer_UpdatesSelectedOptionAndReturnsTrue()
        {
            var sessionId = Guid.NewGuid();
            var questionId = Guid.NewGuid();
            var oldOptionId = Guid.NewGuid();
            var newOptionId = Guid.NewGuid();

            var existingAnswer = new UserAnswer
            {
                QuizSessionId = sessionId,
                QuestionId = questionId,
                SelectedOptionId = oldOptionId
            };

            var session = new QuizSession
            {
                Id = sessionId,
                UserAnswers = new List<UserAnswer> { existingAnswer }
            };

            _quizSessionRepositoryMock.Setup(r => r.GetSessionWithDetailsAsync(sessionId)).ReturnsAsync(session);

            var result = await _quizSessionService.SubmitAnswerAsync(sessionId, questionId, newOptionId);

            Assert.True(result);
            Assert.Single(session.UserAnswers);
            Assert.Equal(newOptionId, existingAnswer.SelectedOptionId);

        }

        [Fact]
        public async Task GetTeacherMultiplayerResultsAsync_ValidFilterAndPagination_ReturnsCorrectDto()
        {
            var teacherId = Guid.NewGuid();

            var sessions = new List<QuizSession> {
                    new QuizSession{
                    Id = Guid.NewGuid(),
                    Quiz = new Quiz { Title = "Math Basics" },
                    Student = new Student { Name = "Dima" },
                    GroupName = "Group A",
                    Score = 100,
                    FinishedAt = DateTime.UtcNow}

            };

            var inputDto = new MultiplayerGamesResultsDTO
            {
                title_filter = "Math",
                universalDTO = new UniversalDTO
                {
                    sortField = "Title",
                    sortOrder = SortOrder.Descending, // "Math Advanced", затем "Math Basics"
                    PageNumber = 1,
                    PageSize = 1
                }
            };
            _quizSessionRepositoryMock.Setup(r => r.GetSessionsByTeacherAsync(teacherId)).ReturnsAsync(sessions);

            var result = await _quizSessionService.GetTeacherMultiplayerResultsAsync(inputDto, teacherId);

            Assert.Equal("Math", result.title_filter);
            Assert.NotNull(result.universalDTO);
            Assert.Equal(1, result.universalDTO.TotalCount);

            var game = result.userGames.First();
            Assert.Equal("Math Basics", game.QuizTitle);
            Assert.Equal("Dima", game.StudentName);
            Assert.Equal("Group A", game.GroupName);
            Assert.Equal(100, game.Score);
        }

        [Fact]
        public async Task GetTeacherMultiplayerResultsAsync_NoMatchingFilter_ReturnsEmptyListAndZeroCount()
        {
            var teacherId = Guid.NewGuid();

            var sessions = new List<QuizSession> { new QuizSession {
            Id = Guid.NewGuid(),
            Quiz = new Quiz { Title = "Physics" },
            Student = new Student { Name = "Bob" },
            GroupName = "Group A" }};

            var inputDto = new MultiplayerGamesResultsDTO
            {
                title_filter = "nonExist",
                universalDTO = new UniversalDTO { PageNumber = 1, PageSize = 10 }
            };

            _quizSessionRepositoryMock.Setup(r => r.GetSessionsByTeacherAsync(teacherId)).ReturnsAsync(sessions);

            var result = await _quizSessionService.GetTeacherMultiplayerResultsAsync(inputDto, teacherId);

            Assert.NotNull(result);
            Assert.Equal(0, result.universalDTO.TotalCount);
        }

        [Fact]
        public async Task GetQuizResultAsync_ValidFilterAndPagination_ReturnsCorrectDto()
        {
            var sessionId = Guid.NewGuid();
            var question1Id = Guid.NewGuid();
            var correctOptionId = Guid.NewGuid();
            var wrongOptionId = Guid.NewGuid();

            var question1 = new Question
            {
                Id = question1Id,
                Title = "Q1",
                Description = "Desc 1",
                AnswerOptions = new List<AnswerOption> {
            new AnswerOption { Id = correctOptionId, Text = "Correct Option", IsCorrect = true },
            new AnswerOption { Id = wrongOptionId, Text = "Wrong Option", IsCorrect = false }}
            };
            var session = new QuizSession
            {
                Id = sessionId,
                FinishedAt = null,
                Quiz = new Quiz
                {
                    Title = "Math Quiz",
                    Questions = new List<Question> { question1 }
                },
                UserAnswers = new List<UserAnswer> { new UserAnswer { QuestionId = question1Id, SelectedOptionId = correctOptionId } }
            };
            _quizSessionRepositoryMock.Setup(r => r.GetSessionWithDetailsAsync(sessionId)).ReturnsAsync(session);
            var result = await _quizSessionService.GetQuizResultAsync(sessionId);

            Assert.NotNull(result);
            Assert.Equal("Math Quiz", result.QuizTitle);
            Assert.Equal(100, result.Score);
            Assert.Equal(1, result.CorrectAnswers);
            Assert.Equal(1, result.TotalQuestions);
            Assert.NotNull(session.FinishedAt);

            var questionResult = result.Questions.First();
            Assert.True(questionResult.IsCorrect);
        }

        [Fact]
        public async Task GetQuizResultAsync_SessionNotFound_ReturnsNull()
        {
            Guid sessionId = Guid.NewGuid();

            _quizSessionRepositoryMock.Setup(x => x.GetSessionWithDetailsAsync(sessionId)).ReturnsAsync(() => null);

            var result = await _quizSessionService.GetQuizResultAsync(sessionId);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteQuizSessionAsync_SessionExists_DeletesSessionAndSaves()
        {
            Guid sessionId = Guid.NewGuid();

            QuizSession quizSession = new QuizSession
            {
                Id = sessionId,
                Score = 0
            };

            _quizSessionRepositoryMock.Setup(x => x.GetByIdAsync(sessionId)).ReturnsAsync(quizSession);

            await _quizSessionService.DeleteQuizSessionAsync(sessionId);
            _quizSessionRepositoryMock.Verify(x => x.DeleteAsync(sessionId), Times.Once);

        }
        
    }
}
