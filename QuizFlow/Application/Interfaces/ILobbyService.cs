using QuizFlow.Models;

namespace QuizFlow.Application.Interfaces
{
    public interface ILobbyService
    {
        public void CreateLobby(QuizMultiPlayerLobby lobby);
        public QuizMultiPlayerLobby? GetLobby(string roomCode);
        public void AddUserToLobby(string roomCode, LobbyUser user);
        public void RemoveLobby(string roomCode);
    }
}
