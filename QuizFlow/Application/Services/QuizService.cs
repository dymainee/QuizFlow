using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using QuizFlow.Application.Interfaces;
using QuizFlow.DTO;
using QuizFlow.Infrastructure.Interfaces;
using QuizFlow.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using SortOrder = QuizFlow.Models.Enums.SortOrder;

namespace QuizFlow.Application.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;

        public QuizService(IQuizRepository quizRepository) {
            _quizRepository = quizRepository;
        }

        public async Task CreateAsync(Quiz quiz) {
            
            await _quizRepository.CreateAsync(quiz);
            await _quizRepository.SaveChangesAsync();
        }
        public async Task<Quiz?> GetQuizWithQuestionsAsync(Guid id) {
            return await _quizRepository.GetQuestionsAsync(id);
        }
        public async Task AddQuestionsToQuizAsync(AddQuestionDTO dto) {
            var quiz = await _quizRepository.GetQuestionsAsync(dto.Id);
            if (quiz == null) return;
            string? imagePath = null;
            if (dto.ImageFile != null) {
                string getExtension = Path.GetExtension(dto.ImageFile.FileName); // img
                string uniqFileName = Guid.NewGuid() + getExtension;
                string saveFolder = Path.Combine("wwwroot", "Images", "quizzes");
                string fullPath = Path.Combine(saveFolder, uniqFileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream); 
                }
                imagePath = "/Images/quizzes/" + uniqFileName;
            }

            Question question = new Question( title: dto.Title, description: dto.Description, imagePath: imagePath, quizId: dto.Id);
            foreach (var answerDto in dto.Options)
            {
                if (!string.IsNullOrEmpty(answerDto.Text))
                {
                    question.AnswerOptions.Add(new AnswerOption
                    {
                        Text = answerDto.Text,
                        IsCorrect = answerDto.isCorrect,
                        QuestionId = question.Id

                    });

                }
            }
            quiz.Questions.Add(question);

            await _quizRepository.SaveChangesAsync();
        }

    }
}
