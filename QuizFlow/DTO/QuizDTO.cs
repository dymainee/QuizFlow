using QuizFlow.Models;
using QuizFlow.Models.Enums;

namespace QuizFlow.DTO
{
    public class QuizDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TimeSpan TimeLimit { get; set; }
        public IFormFile? ImageFile { get; set; } //file from HTTPRequest
        public Guid TeacherId { get; set; }
    }
}
