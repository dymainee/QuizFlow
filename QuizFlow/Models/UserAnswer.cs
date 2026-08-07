namespace QuizFlow.Models
{
    public class UserAnswer
    { //to do the history of played sessions plus quiz results 
        public Guid Id { get; set; }
        public Guid QuizSessionId { get; set; }
        public QuizSession QuizSession { get; set; } 
        public Guid QuestionId { get; set; }
        public Guid SelectedOptionId { get; set; }
        public bool IsCorrect { get; set; }
        public UserAnswer() { }
    }
}
