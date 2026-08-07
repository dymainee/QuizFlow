namespace QuizFlow.DTO
{
    public class QuizSessionResultDTO
    {
        public string QuizTitle { get; set; } = string.Empty;
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public List<QuestionResultDTO> Questions { get; set; } = new();
    }
    public class QuestionResultDTO
    {
        public string QuestionTitle { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public List<OptionDTO> Options { get; set; } = new List<OptionDTO>();
    }
    public class OptionDTO
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public bool IsSelected { get; set; }
    }
}
