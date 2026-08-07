using QuizFlow.Models.Enums;

namespace QuizFlow.Models
{
   public class Student : User
    {
        public List<QuizSession> QuizSessions { get; set; } = new List<QuizSession>();
        public Student() { }

        public Student(string username, string passwordHash, string email, string name, string surname, DateOnly dateofbirth)
            : base(username, passwordHash, email, UserRole.Student, name, surname, dateofbirth)
        {
        } //Чтобы при создании студента роль UserRole.Student присваивалась автоматически
    }
}
