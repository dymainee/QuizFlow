using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizFlow.Application.Interfaces;
using QuizFlow.DTO;
using System.Security.Claims;

namespace QuizFlow.Controllers
{
    [Authorize]
    public class QuizSessionController : Controller
    {
        private readonly IQuizSessionService _quizSessionService;

        public QuizSessionController(IQuizSessionService quizSessionService)
        {
            _quizSessionService = quizSessionService;
        }
        [HttpPost]
        public async Task<IActionResult> StartQuizSession(Guid quizId)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid currentUser = Guid.Parse(id);
            Guid sessionId = await _quizSessionService.StartSessionAsync(currentUser, quizId);
            return RedirectToAction("GetQuestion", new { sessionId = sessionId, questionNumber = 1 });
        }

        [HttpGet]
        public async Task<IActionResult> GetQuizResult(Guid sessionId)
        {
            QuizSessionResultDTO result = await _quizSessionService.GetQuizResultAsync(sessionId);
            if (result != null)
            {
                return View(result);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetQuestion(Guid sessionId, int questionNumber)
        {
            var questionDto = await _quizSessionService.GetQuestionsAsync(sessionId, questionNumber);
            if (questionDto == null)
            {
                return RedirectToAction("GetQuizResult", new { sessionId });
            }
            return View(questionDto);
        }

        [HttpPost]
        public async Task<IActionResult> GetQuestion(Guid sessionId, Guid questionId, Guid selectedOptionId, int questionNumber)
        {
            var saveAnswer = await _quizSessionService.SubmitAnswerAsync(sessionId, questionId, selectedOptionId);

            return RedirectToAction("GetQuestion", new { sessionId, questionNumber = questionNumber + 1 });
        }
    }
}
