using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuizFlow.DTO
{
    public class RegisterUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
    }

    public class RegisterStudentDto : RegisterUserDto
    {
    }

    public class RegisterTeacherDto : RegisterUserDto
    {
        public string WorkPlace { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
    }

    public class LoginDto() {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

