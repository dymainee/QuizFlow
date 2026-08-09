using QuizFlow.Models;

namespace QuizFlow.DTO
{
    public class ProfileUserDTO
    {
        public Guid id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
    }
    public class UserQuizSessionDTO
    {
        public Guid SessionId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public string? StudentName { get; set; }
        public string? GroupName { get; set; }
        public int Score { get; set; }
        public DateTime? FinishedAt { get; set; }
    }

    public class StudentProfileDTO : ProfileUserDTO
    {
        public List<UserQuizSessionDTO> userGames { get; set; } = new List<UserQuizSessionDTO>();
        public UniversalDTO universalDTO { get; set; } = new UniversalDTO();
        public string? title_filter { get; set; }
    }

    public class TeacherProfileDTO : ProfileUserDTO
    {
        public string WorkPlace { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public List<Quiz> Quizzes { get; set; } = new List<Quiz>(); //N + 1 better to create anoher DTO List<TeacherQuizes>
        public UniversalDTO universalDTO { get; set; } = new UniversalDTO();
        public string? title_filter { get; set; }
    }

}

