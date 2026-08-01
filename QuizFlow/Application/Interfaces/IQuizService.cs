using QuizFlow.DTO;
using QuizFlow.Models;

namespace QuizFlow.Application.Interfaces
{
    public interface IQuizService
    {
        public Task CreateAsync(Quiz quiz);
        public Task AddQuestionsToQuizAsync(AddQuestionDTO dto);
        public Task<Quiz?> GetQuizWithQuestionsAsync(Guid id);

    }
}
