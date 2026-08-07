using QuizFlow.Application.Interfaces;
using QuizFlow.DTO;
using QuizFlow.Infrastructure.Interfaces;
using QuizFlow.Models;
using QuizFlow.Models.Enums;

namespace QuizFlow.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHashService _hashService;
        private readonly IJwtProvider _jwtProvider;

        public UserService(IUserRepository userRepository, IHashService hashService, IJwtProvider jwtProvider)
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
            if (user == null)
            {
                throw new Exception("Invalid email or password"); 
            }
            var result = _hashService.Verify(dto.Password, user.PasswordHash);
            if (result == false) {
                throw new Exception("Failed to login");
            }
            var token = _jwtProvider.GenerateToken(user);

            return token;
        }

        public async Task<TeacherProfileDTO> GetTeacherProfileAsync(Guid id, TeacherProfileDTO inputDto) {
            Teacher? teacher = await _userRepository.GetAllQuizzesAsync(id); //casting as Teacher 

            IEnumerable<Quiz> filteredQuizzes = teacher.Quizzes;
            if (!string.IsNullOrEmpty(inputDto.title_filter)) {
                filteredQuizzes = filteredQuizzes.Where(x => x.Title.Contains(inputDto.title_filter));
            }
            filteredQuizzes = (inputDto.universalDTO.sortField, inputDto.universalDTO.sortOrder) switch
            {
                ("Published", SortOrder.Descending) => filteredQuizzes.OrderByDescending(x => x.Status == QuizStatus.Published),
                ("Published", _) => filteredQuizzes.OrderBy(x => x.Status == QuizStatus.Published),
                ("Archived", SortOrder.Descending) => filteredQuizzes.OrderByDescending(x => x.Status == QuizStatus.Archived),
                ("Archived", _) => filteredQuizzes.OrderBy(x => x.Status == QuizStatus.Archived),
                ("Title", SortOrder.Descending) => filteredQuizzes.OrderByDescending(x => x.Title),
                ("Title", _) => filteredQuizzes.OrderBy(x => x.Title),
                ("Date", SortOrder.Descending) => filteredQuizzes.OrderByDescending(x => x.CreatedAt),
                ("Date", _) => filteredQuizzes.OrderBy(x => x.CreatedAt),

                _ => filteredQuizzes.OrderByDescending(x => x.CreatedAt)
            };



            TeacherProfileDTO dto = new TeacherProfileDTO
            {
                id = teacher.Id,
                Username = teacher.Username,
                Email = teacher.Email,
                Name = teacher.Name,    
                Surname = teacher.Surname,
                Password = teacher.PasswordHash,
                DateOfBirth = teacher.DateOfBirth,
                WorkPlace = teacher.WorkPlace,
                Specialization = teacher.Specialization,

                Quizzes = filteredQuizzes.ToList(),
                universalDTO = inputDto.universalDTO,
                title_filter = inputDto.title_filter

            };
            dto.universalDTO.TotalCount = filteredQuizzes.Count();

            dto.Quizzes = filteredQuizzes
                   .Skip((dto.universalDTO.PageNumber - 1) * dto.universalDTO.PageSize)
                   .Take(dto.universalDTO.PageSize)
                   .ToList();

            return dto;

        }

        public async Task<StudentProfileDTO> GetStudentProfileAsync(Guid Id, StudentProfileDTO inputDto)
        {
            //Student? student = await _userRepository.GetByIdAsync(Id) as Student; //casting 
            Student? student = await _userRepository.GetStudentWithSessionsAsync(Id);
            if (student == null)
            {
                return null;
            }
            var filteredSessions = student.QuizSessions
                    .Where(x => x.FinishedAt != null)
                    .AsEnumerable();
            if (!string.IsNullOrEmpty(inputDto.title_filter))
            {
                filteredSessions = filteredSessions.Where(x => x.Quiz.Title.Contains(inputDto.title_filter));
            }
            filteredSessions = (inputDto.universalDTO?.sortField, inputDto.universalDTO?.sortOrder) switch
            {
                ("Title", SortOrder.Descending) => filteredSessions.OrderByDescending(x => x.Quiz.Title),
                ("Title", _) => filteredSessions.OrderBy(x => x.Quiz.Title),
                ("Date", SortOrder.Descending) => filteredSessions.OrderByDescending(x => x.FinishedAt),
                ("Date", _) => filteredSessions.OrderBy(x => x.FinishedAt),

                _ => filteredSessions.OrderByDescending(x => x.FinishedAt)
            };
            inputDto.universalDTO.TotalCount = filteredSessions.Count();

            var pagedSessions = filteredSessions
                    .Skip((inputDto.universalDTO.PageNumber - 1) * inputDto.universalDTO.PageSize)
                    .Take(inputDto.universalDTO.PageSize)
                    .ToList();

            StudentProfileDTO dto = new StudentProfileDTO
            {
                id = student.Id,
                Username = student.Username,
                Email = student.Email,
                Name = student.Name,
                Surname = student.Surname,
                DateOfBirth = student.DateOfBirth,
                universalDTO = inputDto.universalDTO,
                title_filter = inputDto.title_filter,

                userGames = pagedSessions.Select(qs => new UserQuizSessionDTO
                {
                    SessionId = qs.Id, 
                    QuizTitle = qs.Quiz.Title,
                    Score = qs.Score,
                    FinishedAt = qs.FinishedAt
                }).ToList()
            };
            return dto;
        }

        public async Task UpdateStudentProfile(Guid id, StudentProfileDTO dto) {
            Student? student = await _userRepository.GetByIdAsync(id) as Student;
            student.Name = dto.Name;
            student.Surname = dto.Surname;
            student.DateOfBirth = dto.DateOfBirth;
            student.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Password)) {
                bool isSamePassword = _hashService.Verify(dto.Password, student.PasswordHash);

                if (!isSamePassword)
                {
                    student.PasswordHash = _hashService.Generate(dto.Password);
                }
            }
            await _userRepository.SaveChangesAsync();
            
        }

        public async Task UpdateTeacherProfile(Guid id,TeacherProfileDTO dto) {
            var teacher = await _userRepository.GetAllTeacherAsync(id);
            teacher.Name = dto.Name;
            teacher.Surname = dto.Surname;
            teacher.DateOfBirth = dto.DateOfBirth;
            teacher.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Password))
            {
                bool isSamePassword = _hashService.Verify(dto.Password, teacher.PasswordHash);

                if (!isSamePassword)
                {
                    teacher.PasswordHash = _hashService.Generate(dto.Password);
                }
            }
            teacher.WorkPlace = dto.WorkPlace;
            teacher.Specialization = dto.Specialization;
            await _userRepository.SaveChangesAsync();
        }






    }
}
