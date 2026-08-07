using QuizFlow.Models;

namespace QuizFlow.Infrastructure.Interfaces
{
    public interface IQuizSessionRepository : IRepository<QuizSession>
    {
        public Task<QuizSession?> GetSessionWithDetailsAsync(Guid id);
        public Task AddAsync(QuizSession quizSession);

    }
}
