namespace QuizFlow.Models
{
    public class QuizMultiPlayerLobby
    {
        public string RoomCode { get; set; } = string.Empty;
        public Guid QuizId { get; set; }
        public Guid TeacherId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public bool IsStarted { get; set; } = false;
        public Guid? SessionId { get; set; } //will be when teacher press the button start
        public List<Guid> ConnectedUsers { get; set; } = new List<Guid>();
    }

}
