using QuizFlow.DTO;

namespace QuizFlow.Application.Interfaces
{
    public interface IMenuService
    {
        public Task<MenuQuizShowDTO> GetAllAsync(MenuQuizShowDTO dto);
    }
}
