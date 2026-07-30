using QuizFlow.Models;

namespace QuizFlow.Infrastructure.Interfaces
{
    public interface IMenuRepository : IRepository<Quiz>
    {
        public Task<List<Quiz>> GetByNameAsync(string title);
    }
}
