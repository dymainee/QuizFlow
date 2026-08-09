using QuizFlow.DTO;

namespace QuizFlow.Application.Interfaces
{
    public interface IQuizSessionService
    {
        public Task<bool> SubmitAnswerAsync(Guid sessionId, Guid questionId, Guid selectedOptionId);
        public Task<QuizSessionResultDTO> GetQuizResultAsync(Guid sessionId);
        public Task<QuestionPageDTO> GetQuestionsAsync(Guid sessionId, int questionNumber);
        public Task<Guid> StartSessionAsync(Guid userId, Guid QuizId, string? groupName);
        public Task DeleteQuizSessionAsync(Guid quizSessionId);
        public Task<MultiplayerGamesResultsDTO> GetTeacherMultiplayerResultsAsync(MultiplayerGamesResultsDTO inputDto, Guid teacherId);

    }
}
