using Microsoft.AspNetCore.Mvc;
using QuizFlow.Application.Interfaces;
using QuizFlow.DTO;
using QuizFlow.Infrastructure.Interfaces;
using QuizFlow.Infrastructure.Repositories;
using QuizFlow.Models;
using QuizFlow.Models.Enums;

namespace QuizFlow.Application.Services
{
    public class QuizSessionService : IQuizSessionService
    {
        private readonly IQuizSessionRepository _quizSessionRepository;

        public QuizSessionService(IQuizSessionRepository quizSessionRepository)
        {
            _quizSessionRepository = quizSessionRepository;
        }
        public async Task<Guid> StartSessionAsync(Guid userId, Guid QuizId, string? groupName)
        {
            var session = new QuizSession
            {
                Id = Guid.NewGuid(),
                QuizId = QuizId,
                GroupName = groupName,
                UserId = userId,
                StartedAt = DateTime.UtcNow,
                Score = 0
            };
            await _quizSessionRepository.AddAsync(session);
            await _quizSessionRepository.SaveChangesAsync();
            return session.Id;
        }
        public async Task<QuestionPageDTO> GetQuestionsAsync(Guid sessionId, int questionNumber)
        {
            var session = await _quizSessionRepository.GetSessionWithDetailsAsync(sessionId);
            if (session == null) return null;
            var questions = session.Quiz.Questions.ToList();
            int totalQuestions = questions.Count;
            if (questionNumber < 1 || questionNumber > totalQuestions)
            {
                return null; 
            }
            var currentQuestion = questions[questionNumber - 1];
            var existingAnswer = session.UserAnswers.FirstOrDefault(x => x.QuestionId == currentQuestion.Id);
            return new QuestionPageDTO
            {
                SessionId = sessionId,
                QuestionId = currentQuestion.Id,
                QuestionTitle = currentQuestion.Title,
                QuestionText = currentQuestion.Description,
                CurrentQuestionNumber = questionNumber,
                Image = currentQuestion.ImagePath,
                TotalQuestions = totalQuestions,
                SelectedOptionId = existingAnswer?.SelectedOptionId, // if we didnt choose we return nothing
                Options = currentQuestion.AnswerOptions.Select(o => new QuestionOptionDTO
                {
                    Id = o.Id,
                    Text = o.Text
                }).ToList()
            };
        }


        public async Task<bool> SubmitAnswerAsync(Guid sessionId, Guid questionId, Guid selectedOptionId)
        {

            var session = await _quizSessionRepository.GetSessionWithDetailsAsync(sessionId);
            if (session == null) return false;


            //we need to have a User Answer to make sure that we did answer on that question (to do step back if we actually need it)
            //plus to store all answers in user profile(history)

            var existinganswer = session.UserAnswers.FirstOrDefault(x => x.QuestionId == questionId);
            if (existinganswer != null)
            {
                existinganswer.SelectedOptionId = selectedOptionId;
            }
            else
            {
                UserAnswer useranswer = new UserAnswer
                {
                    QuizSessionId = sessionId,
                    QuestionId = questionId,
                    SelectedOptionId = selectedOptionId
                };
                session.UserAnswers.Add(useranswer);
            }
            await _quizSessionRepository.SaveChangesAsync();
            return true;
        }

        public async Task<MultiplayerGamesResultsDTO> GetTeacherMultiplayerResultsAsync(MultiplayerGamesResultsDTO inputDto, Guid teacherId)
        {
            var sessions = await _quizSessionRepository.GetSessionsByTeacherAsync(teacherId);
            IEnumerable<QuizSession> filteredSessions = sessions;

            if (!string.IsNullOrEmpty(inputDto.title_filter))
            {
                filteredSessions = filteredSessions.Where(x => x.Quiz.Title.Contains(inputDto.title_filter)
                ||
                (!string.IsNullOrEmpty(x.GroupName) && x.GroupName.Contains(inputDto.title_filter)));
            }

            filteredSessions = (inputDto.universalDTO?.sortField, inputDto.universalDTO?.sortOrder) switch
            {
                ("Title", SortOrder.Descending) => filteredSessions.OrderByDescending(x => x.Quiz.Title),
                ("Title", _) => filteredSessions.OrderBy(x => x.Quiz.Title),
                ("Group", SortOrder.Descending) => filteredSessions.OrderByDescending(x => x.GroupName),
                ("Group", _) => filteredSessions.OrderBy(x => x.GroupName),
                ("Date", SortOrder.Descending) => filteredSessions.OrderByDescending(x => x.FinishedAt),
                ("Date", _) => filteredSessions.OrderBy(x => x.FinishedAt),
                _ => filteredSessions.OrderByDescending(x => x.GroupName)

            }; 
            
            inputDto.universalDTO.TotalCount = filteredSessions.Count();

            var pagedSessions = filteredSessions
                .Skip((inputDto.universalDTO.PageNumber - 1) * inputDto.universalDTO.PageSize)
                .Take(inputDto.universalDTO.PageSize)
                .ToList();
            return new MultiplayerGamesResultsDTO
            {
                universalDTO = inputDto.universalDTO,
                title_filter = inputDto.title_filter,
                userGames = pagedSessions.Select(x => new UserQuizSessionDTO
                {
                    SessionId = x.Id,
                    QuizTitle = x.Quiz.Title,
                    StudentName = x.Student.Name,
                    GroupName = x.GroupName,
                    Score = x.Score,
                    FinishedAt = x.FinishedAt
                }).ToList()
            };
        }


        public async Task<QuizSessionResultDTO> GetQuizResultAsync(Guid sessionId)
        {
            //we will have on the last page a button to finish the test
            var session = await _quizSessionRepository.GetSessionWithDetailsAsync(sessionId);
            if (session == null) return null;
            if(session.FinishedAt == null) session.FinishedAt = DateTime.UtcNow;
            var result = new List<QuestionResultDTO>();

            int correctAnswersCount = 0;

            foreach (var question in session.Quiz.Questions) {
                // находим что ответил юзер
                var useranswer = session.UserAnswers.FirstOrDefault(x => x.QuestionId == question.Id);
                //после чего находим прав ответ(для показ прав ответа)
                //
                var selectedOption = useranswer != null ? question.AnswerOptions.FirstOrDefault(x => x.Id == useranswer.SelectedOptionId) : null;
                bool isCorrect = selectedOption != null && selectedOption.IsCorrect; //нашли ответ проверяем правильно или нет 
                if (isCorrect) correctAnswersCount++;

                var allOptions = question.AnswerOptions.Select(x => new OptionDTO
                {
                    Text = x.Text,
                    IsCorrect = x.IsCorrect,
                    IsSelected = useranswer != null && x.Id == useranswer.SelectedOptionId

                }).ToList();
                

                result.Add(new QuestionResultDTO
                {
                    QuestionTitle = question.Title,
                    QuestionText = question.Description,
                    IsCorrect = isCorrect, 
                    Options = allOptions
                });
                
            }
            int totalQuestions = session.Quiz.Questions.Count;
            int finalScore = totalQuestions > 0 ? (int)Math.Round((double)correctAnswersCount / totalQuestions * 100) : 0;
            session.Score = finalScore;

            await _quizSessionRepository.SaveChangesAsync();
            return new QuizSessionResultDTO
            {
                QuizTitle = session.Quiz.Title,
                Score = finalScore,
                CorrectAnswers = correctAnswersCount,
                TotalQuestions = totalQuestions,
                Questions = result
            };


        }
        public async Task DeleteQuizSessionAsync(Guid quizId)
        {
            var quiz = await _quizSessionRepository.GetByIdAsync(quizId);
            await _quizSessionRepository.DeleteAsync(quizId);
            await _quizSessionRepository.SaveChangesAsync();
        }
    }
}
