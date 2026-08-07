using QuizFlow.Models;

namespace QuizFlow.Infrastructure.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User?> GetByEmailAsync(string email);
        public Task CreateAsync(User user);
        public Task<Teacher?> GetAllQuizzesAsync(Guid id);
        public Task<Teacher?> GetAllTeacherAsync(Guid id);
        public Task<Student?> GetStudentWithSessionsAsync(Guid id);
     }

 }
