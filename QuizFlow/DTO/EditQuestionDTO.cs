namespace QuizFlow.DTO
{
    public class EditQuestionDTO
    {
        public Guid Id { get; set; } 
        public Guid QuizId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ExistingImagePath { get; set; }
        public IFormFile? NewImageFile { get; set; }
        public List<EditAnswerOptionDTO> Options { get; set; } = new List<EditAnswerOptionDTO>();
        public int CorrectAnswerIndex { get; set; }
    }


    public class EditAnswerOptionDTO
    {
        public Guid Id { get; set; } 
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

}
