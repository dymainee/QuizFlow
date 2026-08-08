using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizFlow.Api;
using QuizFlow.Application.Interfaces;
using QuizFlow.Application.Services;
using QuizFlow.DTO;
using QuizFlow.Models;
using System;
using System.Net.Http;
using System.Security.Claims;

namespace QuizFlow.Controllers
{
    [Authorize]
    public class QuizSessionController : Controller
    {
        private readonly IQuizSessionService _quizSessionService;
        private readonly HttpClient _httpClient;
        private readonly ILobbyService _lobbyService;
        private readonly IConfiguration _config;

        public QuizSessionController(IQuizSessionService quizSessionService, HttpClient httpClient, ILobbyService lobbyService, IConfiguration config)
        {
            _quizSessionService = quizSessionService;
            _httpClient = httpClient;
            _lobbyService = lobbyService;
            _config = config;
        }
        [HttpGet]
        [Authorize(Roles = "Teacher")]  
        public IActionResult LobbyHost(Guid quizId) //this method we need to show groupname field
        {
            var lobby = new QuizMultiPlayerLobby
            {
                QuizId = quizId,
            };
            return View("LobbyHost", lobby);
        }
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> LobbyHost(QuizMultiPlayerLobby lobby) //after we are sending all of this and then doing a get to do a multiplayer
        {
            var TeacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid currentTeacher = Guid.Parse(TeacherId);
            string roomCode = Guid.NewGuid().ToString()[..6];
            lobby.RoomCode = roomCode;
            lobby.TeacherId = currentTeacher;

            _lobbyService.CreateLobby(lobby);

            
            return RedirectToAction("ShowLobby", new { roomCode = roomCode }); //= roomCode) $\rightarrow$ подставляется как значение параметра (...=A8F2K9).
        }
        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> ShowLobby(string roomCode) //for tutor we will be showing the qr and people who joined
        {
            var lobby = _lobbyService.GetLobby(roomCode);
            if (lobby == null) return NotFound("did not find a room");
            //генерируем ссылку Request.Scheme Указывает протокол текущего сайта — https или http.

            string joinUrl = Url.Action("JoinLobby", "QuizSession", new { roomCode = lobby.RoomCode }, Request.Scheme);
            string apiKey = _config["QrApi:Key"];

            string apiUrl = $"https://www.qrcoder.co.uk/api/v4/?key={apiKey}&text={joinUrl}";

            var response = await _httpClient.GetAsync(apiUrl);

            response.EnsureSuccessStatusCode();
            byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

            ViewBag.QrCodeBase64 = Convert.ToBase64String(imageBytes);

            return View("ShowLobby", lobby);
        }

        [HttpGet]
        [Authorize]
        public IActionResult JoinLobby(string roomCode) //for student we will be returning waiting
        {
            var lobby = _lobbyService.GetLobby(roomCode);
            if (lobby == null) return NotFound("Could not find a room");

            return View("LobbyStudent", lobby); 
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
