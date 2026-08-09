using Microsoft.AspNetCore.SignalR;
using QuizFlow.Application.Interfaces;
using QuizFlow.Models;
using System.Security.Claims;

namespace QuizFlow.Hubs
{
    public class QuizHub : Hub
    {
        private readonly ILobbyService _lobbyService;

        public QuizHub(ILobbyService lobbyService)
        {
            _lobbyService = lobbyService;
        }
        public async Task JoinLobbyGroup(string roomCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

            if (Context.User.IsInRole("Teacher"))
            {
                return;
            }
            string userName = Context.User?.Identity?.Name;
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid user = Guid.Parse(userId);
            
            _lobbyService.AddUserToLobby(roomCode, user);
            
            await Clients.Group(roomCode).SendAsync("PlayerJoined", userName);
        }
        public async Task StartQuiz(string roomCode)
        {
            await Clients.Group(roomCode).SendAsync("QuizStarted");
        }
    }
}
