using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuizFlow.DTO
{
    public class RegisterUserDto
    {
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Surname { get; init; } = string.Empty;
        public DateOnly DateOfBirth { get; init; }
    }

    public class RegisterStudentDto : RegisterUserDto
    {
    }

    public class RegisterTeacherDto : RegisterUserDto
    {
        public string WorkPlace { get; init; } = string.Empty;
        public string Specialization { get; init; } = string.Empty;
    }

    public class LoginDto() {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

