namespace QuizFlow.Models
{
    public class AnswerOption
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public Question Question { get; set; }
        public Guid QuestionId { get; set; }
        public bool IsCorrect { get; set; }
        public AnswerOption() { }
        public AnswerOption(string text, Guid questionId, bool isCorrect) {
            Text = text;
            QuestionId = questionId;
            IsCorrect = isCorrect;
        }
    }
}
