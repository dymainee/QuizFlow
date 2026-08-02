using QuizFlow.Application.Interfaces;
using QuizFlow.DTO;
using QuizFlow.Infrastructure.Interfaces;
using QuizFlow.Infrastructure.Repositories;
using QuizFlow.Models;
using QuizFlow.Models.Enums;

namespace QuizFlow.Application.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        public MenuService(IMenuRepository menuRepository) {
            _menuRepository = menuRepository;
        }

        public async Task<MenuQuizShowDTO> GetAllAsync(MenuQuizShowDTO dto)
        {
            var quizzes = await _menuRepository.GetAllAsync();

            var filteredQuizzes = quizzes.Where(x => x.Status == QuizStatus.Published);

            if (!string.IsNullOrEmpty(dto.title_filter))
            {
                string searchTitle = dto.title_filter;
                filteredQuizzes = filteredQuizzes.Where(x => x.Title.Contains(searchTitle));
            }

            filteredQuizzes = (dto.sortField, dto.sortOrder) switch
            {
                ("Title", SortOrder.Descending) => filteredQuizzes.OrderByDescending(x => x.Title),
                ("Title", _) => filteredQuizzes.OrderBy(x => x.Title),
                ("Date", SortOrder.Descending) => filteredQuizzes.OrderByDescending(x => x.CreatedAt),
                ("Date", _) => filteredQuizzes.OrderBy(x => x.CreatedAt),

                _ => filteredQuizzes.OrderByDescending(x => x.CreatedAt)
            };

            dto.TotalCount = filteredQuizzes.Count();

            dto.Quizzes = filteredQuizzes
                   .Skip((dto.PageNumber - 1) * dto.PageSize)
                   .Take(dto.PageSize)
                   .ToList();

            return dto;
        }

        public async Task<List<Quiz>> GetAllAsync()
        {
            return await _menuRepository.GetAllAsync();
        }

    }
}
