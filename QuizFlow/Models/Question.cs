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
        //// DO NOT pass Guid.NewGuid() here!
        //If Id is Guid.Empty -> EF Core marks it as NEW, automatically generates a GUID, and runs INSERT.
        // - If Id is already set -> EF Core assumes it EXISTS in DB and tries UPDATE (causing DbUpdateConcurrencyException)
        public Question(string title, string description, string? imagePath, Guid quizId) {
            Title = title;
            Description = description;
            ImagePath = imagePath;
            QuizId = quizId;
        }
    }
}
