using QuizFlow.Models.Enums;

namespace QuizFlow.Models
{
    public class Quiz
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public QuizStatus Status { get; set; } = QuizStatus.Archived;
        public Guid TeacherId { get; set; }
        public Teacher Teacher { get; set; } //navigation property 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public TimeSpan TimeLimit { get; set; }
        public string? ImagePath { get; set; } //может быть без картинки
        public ICollection<Question> Questions { get; set; } = new List<Question>();

        //public int? Grade {get; set;}
        public Quiz() { } //EFcore(Reflection)
        public Quiz(string title, string description, TimeSpan timeLimit, string imagePath, Guid teacherId)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            TimeLimit = timeLimit;
            ImagePath = imagePath;  
            TeacherId = teacherId;
        }
    }
}
