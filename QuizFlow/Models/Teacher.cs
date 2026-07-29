using QuizFlow.Models.Enums;

namespace QuizFlow.Models
{
    public class Teacher : User
    {
        public string WorkPlace { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public Teacher()
        {
            Role = UserRole.Teacher;
        }
        public Teacher(string username, string passwordHash, string email, string name, string surname, DateOnly dateofbirth, string workPlace, string specialization)
            : base(username, passwordHash, email, UserRole.Teacher,name,surname, dateofbirth)
        {
            WorkPlace = workPlace;
            Specialization = specialization;
        }
    }
}
