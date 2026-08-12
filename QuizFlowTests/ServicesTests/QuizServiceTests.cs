using Moq;
using QuizFlow.Application.Interfaces;
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
    public class QuizServiceTests
    {
        private readonly Mock<IQuizRepository> _quizRepositoryMock;
        private readonly QuizService _quizService;
        public QuizServiceTests()
        {
            _quizRepositoryMock = new Mock<IQuizRepository>();
            _quizService = new QuizService(_quizRepositoryMock.Object);//то готовый экземпляр интерфейса
        }

        [Fact]
        public async Task CreateAsync_ValidQuiz_CallsRepositoryAndSaves()
        {
            Quiz quiz = new Quiz
            {
                Title = "ewe",
                Description = "weewe",

            };
            await _quizService.CreateAsync(quiz);
            _quizRepositoryMock.Verify(x => x.CreateAsync(It.Is<Quiz>(q => q.Title == "ewe" && q.Description == "weewe")), Times.Once);

            _quizRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
        [Fact]
        public async Task GetQuizWithQuestionsAsync_ValidId_ReturnsQuizWithQuestions()
        {
            Guid quizId = Guid.NewGuid();
            Guid questionId = Guid.NewGuid();
            Quiz quiz = new Quiz
            {
                Id = quizId,
                Questions = new List<Question> { new Question { Id = questionId, Title = "ewqe" } }
            };

            _quizRepositoryMock.Setup(x => x.GetQuestionsAsync(quizId)).ReturnsAsync(quiz); //настройка поведения подделки (мока).
            //"Слушай, когда сервис вызовет у тебя метод GetQuestionsAsync с вот этим конкретным quizId,
            //не ищи ничего в реальной базе — сразу верни готовую переменную quiz, которую я создал выше в тесте.
            var result = await _quizService.GetQuizWithQuestionsAsync(quizId);
            Assert.NotNull(result);
            Assert.Equal(quizId, result.Id);
            Assert.NotEmpty(result.Questions);
            Assert.Equal(questionId, result.Questions.First().Id);

        }

        [Fact]
        public async Task AddQuestionsToQuizAsync_ValidDtoWithoutFile_AddsQuestionAndSaves()
        {
            var quizId = Guid.NewGuid();

            var quiz = new Quiz
            {
                Id = quizId,
                Questions = new List<Question>()
            };
            var dto = new AddQuestionDTO
            {
                Id = quizId,
                Title = "New Question",
                Description = "Description",
                ImageFile = null,
                Options = new List<AddAnswerOptionDTO> {
                new AddAnswerOptionDTO { Text = "Option 1", isCorrect = true },
                new AddAnswerOptionDTO { Text = "Option 2", isCorrect = false }}
            };

            _quizRepositoryMock.Setup(r => r.GetQuestionsAsync(quizId)).ReturnsAsync(quiz);

            await _quizService.AddQuestionsToQuizAsync(dto);

            var createdQuestion = quiz.Questions.First();
            Assert.Equal("New Question", createdQuestion.Title);
            Assert.Equal("Description", createdQuestion.Description);
            Assert.Null(createdQuestion.ImagePath);
            Assert.Equal(2, createdQuestion.AnswerOptions.Count);

        }

        [Fact]
        public async Task GetQuestionForEditAsync_ValidQuestionId_ReturnsEditQuestionDto()
        {
            // 1. ARRANGE
            var quizId = Guid.NewGuid();
            var questionId = Guid.NewGuid();
            var option1Id = Guid.NewGuid();
            var option2Id = Guid.NewGuid();

            var question = new Question
            {
                Id = questionId,
                Title = "Test Question",
                Description = "Test Description",
                ImagePath = "/Images/test.jpg",
                AnswerOptions = new List<AnswerOption>{
                    new AnswerOption { Id = option1Id, Text = "Wrong", IsCorrect = false },
                    new AnswerOption { Id = option2Id, Text = "Correct", IsCorrect = true }}
            };

            var quiz = new Quiz
            {
                Id = quizId,
                Questions = new List<Question> { question }
            };

            _quizRepositoryMock
                .Setup(r => r.GetQuestionsAsync(quizId))
                .ReturnsAsync(quiz);
            var result = await _quizService.GetQuestionForEditAsync(questionId, quizId);

            Assert.NotNull(result);
            Assert.Equal(questionId, result.Id);
            Assert.Equal(quizId, result.QuizId);
            Assert.Equal("Test Question", result.Title);
            Assert.Equal("Test Description", result.Description);

            Assert.Equal(2, result.Options.Count);
            Assert.Equal("Wrong", result.Options[0].Text);
        }

        [Fact]
        public async Task GetQuestionForEditAsync_QuestionNotFound_ReturnsNull()
        {
            var quizId = Guid.NewGuid();
            var nonExistingQuestionId = Guid.NewGuid();

            var quiz = new Quiz
            {
                Id = quizId,
                Questions = new List<Question>() // Пустой список вопросов
            };

            _quizRepositoryMock
                .Setup(r => r.GetQuestionsAsync(quizId))
                .ReturnsAsync(quiz);

            var result = await _quizService.GetQuestionForEditAsync(nonExistingQuestionId, quizId);
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateQuestionAsync_ValidDto_UpdatesQuestionAndOptions()
        {
            var quizId = Guid.NewGuid();
            var questionId = Guid.NewGuid();
            var optionId = Guid.NewGuid();

            var existingQuestion = new Question
            {
                Id = questionId,
                Title = "Old Title",
                Description = "Old Description",
                AnswerOptions = new List<AnswerOption> { new AnswerOption { Id = optionId, Text = "Old Option Text", IsCorrect = false } }
            };

            var quiz = new Quiz
            {
                Id = quizId,
                Questions = new List<Question> { existingQuestion }
            };

            var dto = new EditQuestionDTO
            {
                Id = questionId,
                QuizId = quizId,
                Title = "New Title",
                Description = "New Description",
                NewImageFile = null,
                Options = new List<EditAnswerOptionDTO> { new EditAnswerOptionDTO { Id = optionId, Text = "Updated Option Text", IsCorrect = true } }
            };

            _quizRepositoryMock
                .Setup(r => r.GetQuestionsAsync(quizId))
                .ReturnsAsync(quiz);

            await _quizService.UpdateQuestionAsync(dto);

            Assert.Equal("New Title", existingQuestion.Title);
            Assert.Equal("New Description", existingQuestion.Description);

            var updatedOption = existingQuestion.AnswerOptions.First();
            Assert.Equal("Updated Option Text", updatedOption.Text);
            Assert.True(updatedOption.IsCorrect);
        }

        [Fact]
        public async Task DeleteQuestionAsync_QuestionExists_RemovesQuestionAndSaves()
        {
            var quizId = Guid.NewGuid();
            var questionId = Guid.NewGuid();

            var question = new Question
            {
                Id = questionId,
                Title = "Question to Delete"
            };

            var quiz = new Quiz
            {
                Id = quizId,
                Questions = new List<Question> { question }
            };

            _quizRepositoryMock
                .Setup(r => r.GetQuestionsAsync(quizId))
                .ReturnsAsync(quiz);

            await _quizService.DeleteQuestionAsync(questionId, quizId);

            Assert.Empty(quiz.Questions);


        }
        [Fact]
        public async Task PublishQuizAsync_QuizExists_UpdatesStatusToPublishedAndSaves()
        {
            var quizId = Guid.NewGuid();
            var quiz = new Quiz { Id = quizId, Status = QuizStatus.Archived };

            _quizRepositoryMock
                .Setup(r => r.GetByIdAsync(quizId))
                .ReturnsAsync(quiz);

            await _quizService.PublishQuizAsync(quizId);

            Assert.Equal(QuizStatus.Published, quiz.Status);
            _quizRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ArchiveQuizAsync_QuizExists_UpdatesStatusToArchivedAndSaves()
        {
            var quizId = Guid.NewGuid();
            var quiz = new Quiz { Id = quizId, Status = QuizStatus.Published };

            _quizRepositoryMock
                .Setup(r => r.GetByIdAsync(quizId))
                .ReturnsAsync(quiz);
            await _quizService.ArchiveQuizAsync(quizId);

            Assert.Equal(QuizStatus.Archived, quiz.Status);
            _quizRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
        [Fact]
        public async Task DeleteQuizAsync_QuizExists_DeletesQuizAndSaves()
        {
            var quizId = Guid.NewGuid();
            var quiz = new Quiz { Id = quizId, Title = "Test Quiz" };

            _quizRepositoryMock
                .Setup(r => r.GetByIdAsync(quizId))
                .ReturnsAsync(quiz);

            await _quizService.DeleteQuizAsync(quizId);

            _quizRepositoryMock.Verify(r => r.DeleteAsync(quizId), Times.Once);
            _quizRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }


    }
}
   
