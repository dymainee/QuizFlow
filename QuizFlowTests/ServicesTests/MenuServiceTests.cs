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
    public class MenuServiceTests
    {
        public readonly Mock<IMenuRepository> _menuRepositoryMocK;
        public readonly MenuService _menuService;

        public MenuServiceTests()
        {
            _menuRepositoryMocK = new Mock<IMenuRepository>();
            _menuService = new MenuService(_menuRepositoryMocK.Object);
        }
        [Fact]
        public async Task GetAllAsync_ValidFilterAndPagination_ReturnsFilteredAndPagedQuizzes()
        {
            var quizzes = new List<Quiz>{
                new Quiz { Id = Guid.NewGuid(), Title = "C# Basics", Status = QuizStatus.Published, CreatedAt = DateTime.UtcNow.AddDays(-2) },
                new Quiz { Id = Guid.NewGuid(), Title = "C# Advanced", Status = QuizStatus.Published, CreatedAt = DateTime.UtcNow.AddDays(-1) },
                new Quiz { Id = Guid.NewGuid(), Title = "SQL Basics", Status = QuizStatus.Published, CreatedAt = DateTime.UtcNow }};

            var inputDto = new MenuQuizShowDTO
            {
                title_filter = "C#",
                sortField = "Title",
                sortOrder = SortOrder.Descending,
                PageNumber = 1,
                PageSize = 10
            };

            _menuRepositoryMocK
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(quizzes);

            // 2. ACT
            var result = await _menuService.GetAllAsync(inputDto);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Quizzes.Count);

            Assert.Equal("C# Basics", result.Quizzes[0].Title);
        }
        [Fact]
        public async Task GetAllAsync_FiltersOutUnpublishedQuizzes()
        {
            var quizzes = new List<Quiz>{
                new Quiz { Id = Guid.NewGuid(), Title = "C# Basics", Status = QuizStatus.Published, CreatedAt = DateTime.UtcNow.AddDays(-2) },
                new Quiz { Id = Guid.NewGuid(), Title = "C# Advanced", Status = QuizStatus.Archived, CreatedAt = DateTime.UtcNow.AddDays(-1) },
                new Quiz { Id = Guid.NewGuid(), Title = "SQL Basics", Status = QuizStatus.Archived, CreatedAt = DateTime.UtcNow }};

            var inputDto = new MenuQuizShowDTO
            {
                PageNumber = 1,
                PageSize = 10
            };

            _menuRepositoryMocK
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(quizzes);

            var result = await _menuService.GetAllAsync(inputDto);

            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Quizzes);
            Assert.Equal("C# Basics", result.Quizzes.First().Title);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllQuizzesFromRepository()
        {
            var expectedQuizzes = new List<Quiz>{new Quiz { Id = Guid.NewGuid(), Title = "Quiz 1" },new Quiz { Id = Guid.NewGuid(), Title = "Quiz 2" }};

            _menuRepositoryMocK
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(expectedQuizzes);
            //[Строка 1] Setup: "Когда у тебя спросят GetAllAsync, ответь: expectedQuizzes"

            var result = await _menuService.GetAllAsync();

            //[Строка 2] Act: _menuService вызывает _menuRepository.GetAllAsync()и благодаря
            //Setup ПОЛУЧАЕТ тот самый expectedQuizzes!

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(expectedQuizzes, result); 

            _menuRepositoryMocK.Verify(r => r.GetAllAsync(), Times.Once);
        }
    }
}
