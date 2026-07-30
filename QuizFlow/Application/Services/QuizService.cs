using Microsoft.Data.SqlClient;
using QuizFlow.Application.Interfaces;
using QuizFlow.DTO;
using QuizFlow.Infrastructure.Interfaces;
using QuizFlow.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using SortOrder = QuizFlow.Models.Enums.SortOrder;

namespace QuizFlow.Application.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;

        public QuizService(IQuizRepository quizRepository) {
            _quizRepository = quizRepository;
        }

        public async Task CreateAsync(Quiz quiz) {
            
            await _quizRepository.CreateAsync(quiz);
            await _quizRepository.SaveChangesAsync();
        }

    }
}
