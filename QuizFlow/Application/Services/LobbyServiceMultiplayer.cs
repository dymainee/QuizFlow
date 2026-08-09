using QuizFlow.Application.Interfaces;
using QuizFlow.Models;

namespace QuizFlow.Application.Services
{
    public class LobbyServiceMultiplayer : ILobbyService
    {
        private readonly List<QuizMultiPlayerLobby> _lobbies = new List<QuizMultiPlayerLobby>();
        private readonly object _lock = new object();
        public void CreateLobby(QuizMultiPlayerLobby lobby)
        {
            lock (_lock)
            {
                _lobbies.Add(lobby);
            }
        }
        public QuizMultiPlayerLobby? GetLobby(string roomCode)
        {
            lock (_lock)
            {
                return _lobbies.FirstOrDefault(x => x.RoomCode == roomCode);
            }
        }
        public void AddUserToLobby(string roomCode, Guid userId)
        {
            lock (_lock)
            {
                var lobby = _lobbies.FirstOrDefault(x => x.RoomCode == roomCode); //x — это абстрактное имя для одного конкретного элемента (модели)
                if (lobby != null && !lobby.ConnectedUsers.Contains(userId))
                {
                    lobby.ConnectedUsers.Add(userId);
                }
            }
        }
        public void RemoveLobby(string roomCode)
        {
            lock (_lock)
            {
                _lobbies.RemoveAll(x => x.RoomCode == roomCode);
            }
        }
    }
}
