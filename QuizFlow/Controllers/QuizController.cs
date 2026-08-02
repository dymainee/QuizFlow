using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizFlow.Application.Interfaces;
using QuizFlow.DTO;
using QuizFlow.Infrastructure.Interfaces;
using QuizFlow.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuizFlow.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid currentTeacherID = Guid.Parse(id); //(текст) в объект типа Guid
            var dto = new QuizDTO
            {
                TeacherId = currentTeacherID
            };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(QuizDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            string? imagePath = null;

            if (dto.ImageFile != null)
            {
                // Path правильно и безопасно работать со строками путей к файлам и папкам.
                string extension = Path.GetExtension(dto.ImageFile.FileName); //Вырезаем расширение (.jpg, .png) и генерируем уникальное имя
                string uniqFileName = Guid.NewGuid().ToString() + extension;
                // wwwroot  Все файлы, которые лежат внутри wwwroot, становятся доступны пользователям через браузер.
                string saveFolder = Path.Combine("wwwroot", "Images", "quizzes"); // склеивает имена папок в корректный путь

                string fullPath = Path.Combine(saveFolder, uniqFileName); //"wwwroot\Images\quizzes\a1b2c3d4-5678.jpg"

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream); //в этот новый файл.
                }
                //В Базу Данных мы сохраняем путь для браузера
                imagePath = "/Images/quizzes/" + uniqFileName; //это веб-адрес (URL),
                //ASP.NET Core принимает этот запрос, автоматически подставляет wwwroot перед адресом,
                //находит файл wwwroot/Images/quizzes/photo.jpg на диске и отдаёт его картинкой пользователю.
            }

            Quiz quiz = new Quiz(title: dto.Title, description: dto.Description, timeLimit: dto.TimeLimit, imagePath: imagePath, teacherId: dto.TeacherId);
            await _quizService.CreateAsync(quiz);
            return RedirectToAction("AddQuestion", new {quizId = quiz.Id});
        }

        [HttpGet]
        public async Task<IActionResult> AddQuestion(Guid quizId) {
            var quiz = await _quizService.GetQuizWithQuestionsAsync(quizId);
            if (quiz == null) return NotFound();
            var dto = new AddQuestionDTO
            {
                Id = quizId,
                ExistingQuestions = quiz.Questions.ToList()
            };
            return View(dto); 
        }

        [HttpPost]
        public async Task<IActionResult> AddQuestion(AddQuestionDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            for (int i = 0; i < dto.Options.Count; i++) {
                dto.Options[i].isCorrect = (i == dto.CorrectAnswerIndex);
            }
            await _quizService.AddQuestionsToQuizAsync(dto);
            return RedirectToAction("AddQuestion", new { quizId = dto.Id }); //quizId обратно в Get-метод, чтобы страница перезагрузилась
        }
        
        [HttpGet]
        public async Task<IActionResult> EditQuestion(Guid quizId, Guid Id) {
            var dto = await _quizService.GetQuestionForEditAsync(Id, quizId);
            if (dto == null) return NotFound();
            
            return View(dto);
        }
        [HttpPost]
        public async Task<IActionResult> EditQuestion(EditQuestionDTO dto) {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            await _quizService.UpdateQuestionAsync(dto);
            return RedirectToAction("AddQuestion", new { quizId = dto.QuizId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteQuestionWithAnswers(Guid quizId, Guid Id) {
            await _quizService.DeleteQuestionAsync(Id, quizId);
            return RedirectToAction("AddQuestion", new { quizId = quizId });
        }

        [HttpPost]
        public async Task<IActionResult> PublishQuiz(Guid quizId) {
            await _quizService.PublishQuizAsync(quizId);
            return RedirectToAction("ShowAll", "Menu");
        }
       
        public IActionResult ArchiveQuiz(Guid quizId)
        {
            return RedirectToAction("ShowAll", "Menu");
        }
    }
}
