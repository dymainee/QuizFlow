using QuizFlow.Application.Services;
using QuizFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizFlowTests.ServicesTests
{
    public class LobbyServiceMultiplayerTests
    {
        private readonly LobbyServiceMultiplayer _lobbyServiceMultiplayer;

        public LobbyServiceMultiplayerTests()
        {
            _lobbyServiceMultiplayer = new LobbyServiceMultiplayer();
        }
        [Fact]
        public void CreateLobby_And_GetLobby_ValidRoomCode_ReturnsLobby()
        {
            var roomCode = "ROOM123";
            var lobby = new QuizMultiPlayerLobby { RoomCode = roomCode };

            _lobbyServiceMultiplayer.CreateLobby(lobby);
            var result = _lobbyServiceMultiplayer.GetLobby(roomCode);

            Assert.NotNull(result);
            Assert.Equal(roomCode, result.RoomCode);
        }

        [Fact]
        public void GetLobby_NonExistingRoomCode_ReturnsNull()
        {
            var result = _lobbyServiceMultiplayer.GetLobby("None");

            Assert.Null(result);
        }

        [Fact]
        public void AddUserToLobby_ValidLobby_AddsUserOnlyOnce()
        {
            var roomCode = "ROOM123";
            var userId = Guid.NewGuid();
            var lobby = new QuizMultiPlayerLobby
            {
                RoomCode = roomCode,
                ConnectedUsers = new List<Guid>()
            };

            _lobbyServiceMultiplayer.CreateLobby(lobby);

            _lobbyServiceMultiplayer.AddUserToLobby(roomCode, userId);
            _lobbyServiceMultiplayer.AddUserToLobby(roomCode, userId);

            var updatedLobby = _lobbyServiceMultiplayer.GetLobby(roomCode);
            Assert.NotNull(updatedLobby);
            Assert.Single(updatedLobby.ConnectedUsers);
        }

        [Fact]
        public void RemoveLobby_ExistingRoomCode_RemovesLobbyFromList()
        {
            var roomCode = "ROOM123";
            var lobby = new QuizMultiPlayerLobby { RoomCode = roomCode };
            _lobbyServiceMultiplayer.CreateLobby(lobby);
            _lobbyServiceMultiplayer.RemoveLobby(roomCode);

            var result = _lobbyServiceMultiplayer.GetLobby(roomCode);
            Assert.Null(result);
        }
    }
}
