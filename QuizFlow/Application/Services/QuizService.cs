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

        public async Task<QuizShowDTO> GetAllAsync(QuizShowDTO dto) {
            var quizzes = await _quizRepository.GetAllAsync(); //!Fix need to do filter in sql and then filter the value
            
            if (!string.IsNullOrEmpty(dto.title_filter))
            {
                string searchTitle = dto.title_filter;
                quizzes = quizzes.Where(x => x.Title.Contains(searchTitle)).ToList();
            }

            quizzes = (dto.sortField, dto.sortOrder) switch
            {
                ("Title", SortOrder.Descending) => quizzes.OrderByDescending(x => x.Title).ToList(),
                ("Title", _) => quizzes.OrderBy(x => x.Title).ToList(),
                ("Date", SortOrder.Descending) => quizzes.OrderByDescending(x => x.CreatedAt).ToList(),
                ("Date", _) => quizzes.OrderBy(x => x.CreatedAt).ToList(),

                _ => quizzes.OrderByDescending(x => x.CreatedAt).ToList()
            };

             dto.TotalCount = quizzes.Count();
             dto.Quizzes = quizzes
                    .Skip((dto.PageNumber - 1) * dto.PageSize)
                    .Take(dto.PageSize)
                    .ToList();

            return dto;
        }

        public async Task CreateAsync(Quiz quiz) {
            
            await _quizRepository.CreateAsync(quiz);
            await _quizRepository.SaveChangesAsync();
        }

        public async Task<List<Quiz>> GetAllAsync() {
            return await _quizRepository.GetAllAsync();
        }

        


    }
}
