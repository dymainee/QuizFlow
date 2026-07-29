using Microsoft.Extensions.Options;
using QuizFlow.Infrastructure.Authentication;
using QuizFlow.Models;

namespace QuizFlow.Application.Interfaces
{
    public interface IJwtProvider
    {
        public string GenerateToken(User user);
    }
}
