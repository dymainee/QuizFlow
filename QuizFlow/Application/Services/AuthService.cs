using QuizFlow.Application.Interfaces;
using QuizFlow.DTO;
using QuizFlow.Infrastructure.Interfaces;
using QuizFlow.Models;

namespace QuizFlow.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHashService _hashService;
        private readonly IJwtProvider _jwtProvider;

        public AuthService(IUserRepository userRepository, IHashService hashService, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _hashService = hashService;
            _jwtProvider = jwtProvider;
        }

        public async Task RegisterAsync(RegisterStudentDto dto) {
            var hashedPassword = _hashService.Generate(dto.Password);
            var student = new Student(dto.Username, dto.Email, hashedPassword, dto.Name, dto.Surname, dto.DateOfBirth);
            await _userRepository.CreateAsync(student);
            await _userRepository.SaveChangesAsync();

        }

        public async Task RegisterAsync(RegisterTeacherDto dto)
        {
            var hashedPassword = _hashService.Generate(dto.Password);
            var teacher = new Teacher(
                dto.Username,
                dto.Email,
                hashedPassword,
                dto.Name,
                dto.Surname,
                dto.DateOfBirth,
                dto.WorkPlace,
                dto.Specialization
            );
            await _userRepository.CreateAsync(teacher);
            await _userRepository.SaveChangesAsync();

        }

        public async Task<string> LoginAsync(LoginDto dto) {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            var result = _hashService.Verify(dto.Password, user.PasswordHash);
            if (result == false) {
                throw new Exception("Failed to login");
            }
            var token = _jwtProvider.GenerateToken(user);

            return token;
        }




    }
}
