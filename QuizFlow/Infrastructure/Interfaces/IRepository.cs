using QuizFlow.Models;

namespace QuizFlow.Infrastructure.Interfaces
{
    public interface IRepository<Model> where Model : class
    {
        public Task<Model?> GetByIdAsync(Guid id);
        public Task<List<Model>> GetAllAsync();
        public Task DeleteAsync(Guid id);
        public Task SaveChangesAsync();

    }
}
