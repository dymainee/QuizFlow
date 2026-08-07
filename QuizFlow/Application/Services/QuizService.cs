using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuizFlow.Application.Interfaces;
using QuizFlow.DTO;
using QuizFlow.Infrastructure.Interfaces;
using QuizFlow.Infrastructure.Repositories;
using QuizFlow.Models;
using QuizFlow.Models.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using SortOrder = QuizFlow.Models.Enums.SortOrder;

namespace QuizFlow.Application.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;

        public QuizService(IQuizRepository quizRepository)
        {
            _quizRepository = quizRepository;
        }

        public async Task CreateAsync(Quiz quiz)
        {

            await _quizRepository.CreateAsync(quiz);
            await _quizRepository.SaveChangesAsync();
        }
        public async Task<Quiz?> GetQuizWithQuestionsAsync(Guid id)
        {
            return await _quizRepository.GetQuestionsAsync(id);
        }
        public async Task AddQuestionsToQuizAsync(AddQuestionDTO dto)
        {
            var quiz = await _quizRepository.GetQuestionsAsync(dto.Id);
            if (quiz == null) return;
            string? imagePath = null;
            if (dto.ImageFile != null)
            {
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

            Question question = new Question(title: dto.Title, description: dto.Description, imagePath: imagePath, quizId: dto.Id);
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

        public async Task<EditQuestionDTO> GetQuestionForEditAsync(Guid questionId, Guid quizId)
        {
            var quiz = await _quizRepository.GetQuestionsAsync(quizId);
            var question = quiz.Questions.FirstOrDefault(x => x.Id == questionId);
            if (question == null) return null;
            var options = question.AnswerOptions.Select(x => new EditAnswerOptionDTO
            {
                Id = x.Id,
                Text = x.Text,
                IsCorrect = x.IsCorrect
            }).ToList(); //select Он берет каждый объект x (где x — это отдельный AnswerOption из базы)
            EditQuestionDTO editQuestionDTO = new EditQuestionDTO
            {
                Id = question.Id,
                QuizId = quiz.Id,
                Title = question.Title,
                Description = question.Description,
                ExistingImagePath = question.ImagePath,
                CorrectAnswerIndex = options.FindIndex(x => x.IsCorrect), //найди мне индекс
                Options = options
            };
            return editQuestionDTO;
        }

        public async Task UpdateQuestionAsync(EditQuestionDTO dto)
        {
            var quiz = await _quizRepository.GetQuestionsAsync(dto.QuizId);
            var question = quiz.Questions.FirstOrDefault(x => x.Id == dto.Id);
            question.Title = dto.Title;
            question.Description = dto.Description;
            if (dto.NewImageFile != null)
            {
                string getExtension = Path.GetExtension(dto.NewImageFile.FileName); // img
                string uniqFileName = Guid.NewGuid() + getExtension;
                string saveFolder = Path.Combine("wwwroot", "Images", "quizzes");
                string fullPath = Path.Combine(saveFolder, uniqFileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.NewImageFile.CopyToAsync(stream);
                }
                question.ImagePath = "/Images/quizzes/" + uniqFileName;
            }

            foreach (var option in dto.Options) {
                var existoption = question.AnswerOptions.FirstOrDefault(x => x.Id == option.Id);
                existoption.Text = option.Text;
                existoption.IsCorrect = option.IsCorrect;
            }
            await _quizRepository.SaveChangesAsync();
        }

        public async Task DeleteQuestionAsync(Guid questionId, Guid quizId) {
            var quiz = await _quizRepository.GetQuestionsAsync(quizId);
            if (quiz == null) return;
            var question = quiz.Questions.FirstOrDefault(x => x.Id == questionId);
            if (question == null) return;
            quiz.Questions.Remove(question); 
            await _quizRepository.SaveChangesAsync();
        }
        public async Task PublishQuizAsync(Guid quizId) {
            var quiz =  await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null) return;
            quiz.Status = QuizStatus.Published;
            await _quizRepository.SaveChangesAsync();
        }
        public async Task ArchiveQuizAsync(Guid quizId)
        {
            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null) return;
            quiz.Status = QuizStatus.Archived;
            await _quizRepository.SaveChangesAsync();
        }
        public async Task DeleteQuizAsync(Guid quizId)
        {
            var quiz = await _quizRepository.GetByIdAsync(quizId);
            await _quizRepository.DeleteAsync(quizId);
            await _quizRepository.SaveChangesAsync();
        }
        
    }
}
