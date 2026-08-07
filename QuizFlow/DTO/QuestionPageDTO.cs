namespace QuizFlow.DTO
{
    public class QuestionPageDTO
    {
        public Guid SessionId { get; set; }
        public Guid QuestionId { get; set; }
        public string QuestionTitle { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public string? Image { get; set; }
        public int CurrentQuestionNumber { get; set; }
        public int TotalQuestions { get; set; }
        public Guid? SelectedOptionId { get; set; } //for example if i will return back 

        public List<QuestionOptionDTO> Options { get; set; } = new List<QuestionOptionDTO>();
    }

    public class QuestionOptionDTO
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}

