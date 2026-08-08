namespace QuizFlow.Models
{
    public class QuizSession
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public Quiz Quiz  { get; set; }
        public Student Student { get; set; }
        public Guid UserId { get; set; }
        public int Score { get; set; }
        public string? GroupName { get; set; } // for grouping students in multiplayer
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? FinishedAt { get; set; }
        public List<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
        public QuizSession() { }


    }
}
