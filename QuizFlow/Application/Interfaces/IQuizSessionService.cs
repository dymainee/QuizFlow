using QuizFlow.DTO;

namespace QuizFlow.Application.Interfaces
{
    public interface IQuizSessionService
    {
        public Task<bool> SubmitAnswerAsync(Guid sessionId, Guid questionId, Guid selectedOptionId);
        public Task<QuizSessionResultDTO> GetQuizResultAsync(Guid sessionId);
        public Task<QuestionPageDTO> GetQuestionsAsync(Guid sessionId, int questionNumber);
        public Task<Guid> StartSessionAsync(Guid userId, Guid QuizId);
    }
}
