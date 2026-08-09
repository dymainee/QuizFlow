using Microsoft.EntityFrameworkCore;
using QuizFlow.Data;
using QuizFlow.Infrastructure.Interfaces;
using QuizFlow.Models;

namespace QuizFlow.Infrastructure.Repositories
{
    public class QuizSessionRepository : Repository<QuizSession>, IQuizSessionRepository
    {
        public QuizSessionRepository(ApplicationContext context) : base(context)
        {

        }
        public async Task<QuizSession?> GetSessionWithDetailsAsync(Guid id) {
            return await _modelSet
                .Include(x => x.Quiz)
                    .ThenInclude(x => x.Questions)
                        .ThenInclude(x => x.AnswerOptions)
                .Include(x => x.UserAnswers)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task AddAsync(QuizSession quizSession)
        {
            await _modelSet.AddAsync(quizSession);
        }

        public async Task<List<QuizSession>> GetSessionsByTeacherAsync(Guid teacherId) {
            return await _modelSet
                .Include(x => x.Quiz)
                .Include(x => x.Student)
                .Where(x => x.Quiz.TeacherId == teacherId)
                .ToListAsync();
            
        }


    }
}
