using Microsoft.EntityFrameworkCore;
using QuizFlow.Data;
using QuizFlow.Infrastructure.Interfaces;

namespace QuizFlow.Infrastructure.Repositories
{
    public class Repository<Model> : IRepository<Model> where Model : class
    {
        private readonly ApplicationContext _context;
        protected readonly DbSet<Model> _modelSet;
        public Repository(ApplicationContext context) {
            _context = context; // //знает про все таблицы сразу
            _modelSet = _context.Set<Model>();
        }

        public async Task<Model?> GetByIdAsync(Guid id)
        {
            return await _modelSet.FindAsync(id);
        }

        public async Task<List<Model>> GetAllAsync()
        {
            return await _modelSet.ToListAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _modelSet.FindAsync(id);
            if (entity != null) {
                _modelSet.Remove(entity);
            }
        }
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    }
}
