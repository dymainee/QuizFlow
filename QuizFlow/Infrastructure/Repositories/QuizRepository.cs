using Microsoft.EntityFrameworkCore;
using QuizFlow.Data;
using QuizFlow.Infrastructure.Interfaces;
using QuizFlow.Models;

namespace QuizFlow.Infrastructure.Repositories
{
    public class QuizRepository : Repository<Quiz>, IQuizRepository
    {
        public QuizRepository(ApplicationContext context) : base(context)
        {
        }

        public async Task CreateAsync(Quiz quiz) {
            await _modelSet.AddAsync(quiz);
        }
        
    }
}
