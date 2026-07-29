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


    }
}
