using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuizFlow.Application.Interfaces;
using QuizFlow.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuizFlow.Infrastructure.Authentication
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions _jwtOptions;
        public JwtProvider(IOptions<JwtOptions> options) {
            _jwtOptions = options.Value; // IOptions<T> — это сервис-контейнер .NET.
                                         // Его задача — взять данные из appsettings.json,
                                         // распарсить их и передать готовый экземпляр класса T в сервис
                                         // через Dependency Injection (DI).
        }
        public string GenerateToken(User user) {
            /// Claims — это наборы данных (ключ-значение) о пользователе, 
            // внедряемые в JWT для идентификации и проверки его прав доступа без обращения к базе данных
            //(!Claim принимает ТОЛЬКО строки)
            Claim[] claims = new Claim[]
            { // Он содержит готовые, общепринятые имена (ключи) для стандартных данных пользователя.
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            //Создаем секретный ключ и указываем алгоритм шифрования
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                SecurityAlgorithms.HmacSha256);
            //Формируем сам JWT-токен (указываем подпись и время жизни)
            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: signingCredentials,
                expires: DateTime.UtcNow.AddHours(_jwtOptions.ExpiresHours));
            //Сериализуем объект токена в итоговую компактную строку
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}
