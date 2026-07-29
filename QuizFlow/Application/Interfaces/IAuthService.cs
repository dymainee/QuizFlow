using QuizFlow.DTO;

namespace QuizFlow.Application.Interfaces
{
    public interface IAuthService
    {
        public Task RegisterAsync(RegisterStudentDto dto);

        public Task RegisterAsync(RegisterTeacherDto dto);
        public Task<string> LoginAsync(LoginDto dto);
    }
}
