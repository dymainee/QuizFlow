using Microsoft.EntityFrameworkCore;
using QuizFlow.Data;
using QuizFlow.Infrastructure.Interfaces;
using QuizFlow.Models;

namespace QuizFlow.Infrastructure.Repositories
{
    public class MenuRepository : Repository<Quiz>, IMenuRepository
    {
        public MenuRepository(ApplicationContext context) : base(context)
        {
        }
        public async Task<List<Quiz>> GetByNameAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) ////filter by name 
            {
                return await _modelSet.ToListAsync();
            }
            return await _modelSet
                .Where(x => x.Title.Contains(title))
                .ToListAsync();
        }
    }
}
