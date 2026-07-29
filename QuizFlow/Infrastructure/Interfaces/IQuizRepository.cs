using QuizFlow.Models;

namespace QuizFlow.Infrastructure.Interfaces
{
    public interface IQuizRepository : IRepository<Quiz>
    {
        public Task CreateAsync(Quiz quiz);
        public Task<Quiz?> GetByNameAsync(string title);
    }
}
