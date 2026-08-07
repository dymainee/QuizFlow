using QuizFlow.DTO;

namespace QuizFlow.Application.Interfaces
{
    public interface IUserService
    {
        public Task RegisterAsync(RegisterStudentDto dto);
        public Task RegisterAsync(RegisterTeacherDto dto);
        public Task<string> LoginAsync(LoginDto dto);
        public Task<TeacherProfileDTO> GetTeacherProfileAsync(Guid id, TeacherProfileDTO inputDto);
        public Task<StudentProfileDTO> GetStudentProfileAsync(Guid id, StudentProfileDTO inputDto);
        public Task UpdateStudentProfile(Guid id, StudentProfileDTO dto);
        public Task UpdateTeacherProfile(Guid id, TeacherProfileDTO dto);
    }
}
