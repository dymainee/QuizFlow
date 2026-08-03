using Microsoft.EntityFrameworkCore;
using QuizFlow.Data;
using QuizFlow.Infrastructure.Interfaces;
using QuizFlow.Models;

namespace QuizFlow.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationContext context) : base(context)
        {

        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _modelSet.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task CreateAsync(User user)
        {
            await _modelSet.AddAsync(user);
        }
        public async Task<Teacher?> GetAllQuizzesAsync(Guid id) {
            return await _modelSet
            .OfType<Teacher>() // берёт из базы только те записи, которые являются учителями, и сразу даёт доступ к полям класса
            .Include(x => x.Quizzes)
            .FirstOrDefaultAsync(x => x.Id == id);
            //Генерирует SQL JOIN: Фильтрует
        }
        public async Task<Teacher?> GetAllTeacherAsync(Guid id)
        {
            return await _modelSet
            .OfType<Teacher>() 
            .FirstOrDefaultAsync(x => x.Id == id);
        }


    }
}
