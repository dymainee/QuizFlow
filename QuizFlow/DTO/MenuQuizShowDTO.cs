using QuizFlow.Models;

namespace QuizFlow.DTO
{
    public class MenuQuizShowDTO : UniversalDTO
    {
        public string? title_filter { get; set; }
        public List<Quiz> Quizzes { get; set; } = new List<Quiz>();

    }
}
