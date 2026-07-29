using BCrypt.Net;
using QuizFlow.Application.Interfaces;

namespace QuizFlow.Application.Services
{
    public class HashService : IHashService
    {
        public string Generate(string password) => 
            BCrypt.Net.BCrypt.EnhancedHashPassword(password); //SH 384
        //Чтобы проверить вход, BCrypt берет введенный password,
        //снова хэширует его с солью из hashedPassword и смотрит:
        //получились одинаковые хэши или нет.
        public bool Verify(string password, string hashedPassword) =>
            BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
    }
}
