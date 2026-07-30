using Microsoft.AspNetCore.Identity;
using QuizFlow.Models.Enums;
using System.Data;

namespace QuizFlow.Models
{
    public class Question
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? ImagePath { get; set; }
        public Quiz Quiz { get; set; }  
        public Guid QuizId { get; set; }
        public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
        public Question() { }
        public Question(string title, string description, string? imagePath, Guid quizId) {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            ImagePath = imagePath;
            QuizId = quizId;
        }
    }
}
