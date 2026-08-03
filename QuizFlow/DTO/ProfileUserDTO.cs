using QuizFlow.Models;

namespace QuizFlow.DTO
{
    public class ProfileUserDTO
    {
        public Guid id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
    }

    public class StudentProfileDTO : ProfileUserDTO
    {
    }

    public class TeacherProfileDTO : ProfileUserDTO
    {
        public string WorkPlace { get; init; } = string.Empty;
        public string Specialization { get; init; } = string.Empty;
        public List<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public UniversalDTO universalDTO { get; set; } = new UniversalDTO();
        public string? title_filter { get; set; }
    }

}

