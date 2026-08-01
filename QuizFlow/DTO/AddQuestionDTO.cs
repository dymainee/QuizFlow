using Microsoft.AspNetCore.Mvc.Rendering;
using QuizFlow.Models;

namespace QuizFlow.DTO
{
    public class AddQuestionDTO
    {
        public string Title { get; set; } = string.Empty;   
        public string Description { get; set; } = string.Empty;
        public IFormFile? ImageFile { get; set; }
        public Guid Id { get; set; }
        public List<AddAnswerOptionDTO> Options { get; set; } = new List<AddAnswerOptionDTO>() {
            new AddAnswerOptionDTO(),
            new AddAnswerOptionDTO(),
            new AddAnswerOptionDTO(),
            new AddAnswerOptionDTO()
        };
        public List<Question> ExistingQuestions { get; set; } = new List<Question>();

        public int CorrectAnswerIndex { get; set; }
    }

    public class AddAnswerOptionDTO {
        public string Text { get; set; } = string.Empty;
        public bool isCorrect { get; set; }
    }
}
