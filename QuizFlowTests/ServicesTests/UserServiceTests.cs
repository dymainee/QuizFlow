using Moq;
using QuizFlow.Application.Interfaces;
using QuizFlow.Application.Services;
using QuizFlow.DTO;
using QuizFlow.Infrastructure.Interfaces;
using QuizFlow.Models;
using QuizFlow.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizFlowTests.ServicesTests
{
    public class UserServiceTests
    {// moq Запоминать, какие методы у них вызывали.
        //Возвращать заранее заданный вами результат.
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IHashService> _hashServiceMock;
        private readonly Mock<IJwtProvider> _jwtProviderMock;
        private readonly UserService _userService;
        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _hashServiceMock = new Mock<IHashService>();
            _jwtProviderMock = new Mock<IJwtProvider>();
            _userService = new UserService(
                _userRepositoryMock.Object,
                _hashServiceMock.Object,
                _jwtProviderMock.Object
            );
        }
        [Fact]
        public async Task RegisterAsync_ValidDto_HashesPasswordAndSavesStudent()
        {
            var dto = new RegisterStudentDto
            {
                Username = "student_dima",
                Email = "student@gmail.com",
                Password = "raw_password_123",
                Name = "Dima",
                Surname = "ADawd",
                DateOfBirth = new DateOnly(2005, 4, 8)
            };

            string expectedHash = "hashed_password_xyz";

            _hashServiceMock
            .Setup(x => x.Generate(dto.Password))
            .Returns(expectedHash);

            await _userService.RegisterAsync(dto);

            _hashServiceMock.Verify(x => x.Generate(dto.Password), Times.Once); //проверяет историю вызовов фальшивого объекта

            _userRepositoryMock.Verify(x => x.CreateAsync(It.Is<Student>(s => //(предикат) у нас в тесте нет прямой ссылки на этот конкретный экземпляр в памяти.
            s.Username == dto.Username &&
            s.Email == dto.Email &&
            s.PasswordHash == expectedHash &&
            s.Name == dto.Name &&
            s.Surname == dto.Surname &&
            s.DateOfBirth == dto.DateOfBirth
                                )), Times.Once);

            _userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);


        }
        [Fact]
        public async Task RegisterAsync_ValidDto_HashesPasswordAndSavesTeacher()
        {
            var dto = new RegisterTeacherDto
            {
                Username = "teacher_dima",
                Email = "student@gmail.com",
                Password = "raw_password_123",
                Name = "Dima",
                Surname = "ADawd",
                DateOfBirth = new DateOnly(2005, 4, 8),
                WorkPlace = "KPi",
                Specialization = "History"
            };

            string expectedHash = "hashed_password_xyz";

            _hashServiceMock
            .Setup(x => x.Generate(dto.Password))
            .Returns(expectedHash);

            await _userService.RegisterAsync(dto);

            _hashServiceMock.Verify(x => x.Generate(dto.Password), Times.Once); //проверяет историю вызовов фальшивого объекта

            _userRepositoryMock.Verify(x => x.CreateAsync(It.Is<Teacher>(x => //(предикат) у нас в тесте нет прямой ссылки на этот конкретный экземпляр в памяти.
            x.Username == dto.Username &&
            x.Email == dto.Email &&
            x.PasswordHash == expectedHash &&
            x.Name == dto.Name &&
            x.Surname == dto.Surname &&
            x.DateOfBirth == dto.DateOfBirth &&
            x.WorkPlace == dto.WorkPlace &&
            x.Specialization == dto.Specialization
                                )), Times.Once);

            _userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);


        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsJwtToken()
        {

            var dto = new LoginDto { Email = "test@gmail.com", Password = "correct_password" };
            var user = new Student("username", dto.Email, "hashed_pass", "Name", "Surname", new DateOnly(2000, 1, 1));

            string expectedToken = "valid_jwt_token_xyz";

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

            _hashServiceMock.Setup(x => x.Verify(dto.Password, user.PasswordHash)).Returns(true);

            _jwtProviderMock.Setup(x => x.GenerateToken(user)).Returns(expectedToken);

            var token = await _userService.LoginAsync(dto);

            Assert.NotNull(token);
            Assert.Equal(expectedToken, token);
        }

        [Fact]
        public async Task LoginAsync_UserNotFound_ThrowsException()
        {
            var dto = new LoginDto { Email = "notfound@gmail.com", Password = "password" };
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(() => null);

            var exception = await Assert.ThrowsAsync<Exception>(() => _userService.LoginAsync(dto));
            Assert.Equal("Invalid email or password", exception.Message);


        }
        [Fact]
        public async Task LoginAsync_InvalidPassword_ThrowsException()
        {
            var dto = new LoginDto { Email = "test@gmail.com", Password = "wrong_password" };
            var user = new Student("username", dto.Email, "hashed_pass", "Name", "Surname", new DateOnly(2000, 1, 1));

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync(user);
            _hashServiceMock
            .Setup(h => h.Verify(dto.Password, user.PasswordHash))
            .Returns(false);

            var exception = await Assert.ThrowsAsync<Exception>(() => _userService.LoginAsync(dto));
            Assert.Equal("Failed to login", exception.Message);
        }
        [Fact]
        public async Task GetTeacherProfileAsync_ValidId_ReturnsFilteredAndPaginatedProfile()
        {
            var teacherId = Guid.NewGuid();
            var quizzes = new List<Quiz>
                    {
                        new Quiz { Id = Guid.NewGuid(), Title = "Math 101", CreatedAt = DateTime.UtcNow.AddDays(-1), Status = QuizStatus.Published },
                        new Quiz { Id = Guid.NewGuid(), Title = "Math Advanced", CreatedAt = DateTime.UtcNow, Status = QuizStatus.Published },
                        new Quiz { Id = Guid.NewGuid(), Title = "History Basics", CreatedAt = DateTime.UtcNow, Status = QuizStatus.Archived }
                     };
            var teacher = new Teacher
            {
                Id = teacherId,
                Username = "teacher1",
                Email = "teacher@test.com",
                Quizzes = quizzes
            };
            var inputDto = new TeacherProfileDTO
            {
                title_filter = "Math",
                universalDTO = new UniversalDTO
                {
                    sortField = "Title",
                    sortOrder = SortOrder.Descending,
                    PageNumber = 1,
                    PageSize = 1
                }
            };

            _userRepositoryMock
                .Setup(r => r.GetAllQuizzesAsync(teacherId))
                     .ReturnsAsync(teacher);

            var result = await _userService.GetTeacherProfileAsync(teacherId, inputDto);

            Assert.NotNull(result);
            Assert.Equal(teacher.Username, result.Username);
            Assert.Equal(2, result.universalDTO.TotalCount);
            Assert.Equal("Math Advanced", result.Quizzes.First().Title);

        }
        [Fact]
        public async Task GetTeacherProfileAsync_TeacherNotFound_ThrowsNullReferenceException()
        {
            var teacherId = Guid.NewGuid();
            var inputDto = new TeacherProfileDTO { universalDTO = new UniversalDTO() };

            _userRepositoryMock
                .Setup(r => r.GetAllQuizzesAsync(teacherId))
                .ReturnsAsync(() => null);

            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _userService.GetTeacherProfileAsync(teacherId, inputDto));
        }

        [Fact]
        public async Task GetStudentProfileAsync_ValidId_ReturnsFilteredAndPaginatedProfile()
        {
            var studentId = Guid.NewGuid();
            var student = new Student
            {
                Id = studentId,
                Username = "student_alex",
                Email = "alex@test.com",
                Name = "Alex",
                Surname = "Smith",
                QuizSessions = new List<QuizSession>
                {
                        new QuizSession { Id = Guid.NewGuid(), Quiz = new Quiz { Title = "C# Basics" }, Score = 80, FinishedAt = DateTime.UtcNow.AddDays(-2) },
                        new QuizSession { Id = Guid.NewGuid(), Quiz = new Quiz { Title = "C# Advanced" }, Score = 95, FinishedAt = DateTime.UtcNow.AddDays(-1) },
                        new QuizSession { Id = Guid.NewGuid(), Quiz = new Quiz { Title = "History 101" }, Score = 60, FinishedAt = DateTime.UtcNow }
                }
            };

            var inputDto = new StudentProfileDTO
            {
                title_filter = "C#",
                universalDTO = new UniversalDTO
                {
                    sortField = "Score",
                    sortOrder = SortOrder.Descending,
                    PageNumber = 1,
                    PageSize = 1
                }
            };

            _userRepositoryMock
        .       Setup(r => r.GetStudentWithSessionsAsync(studentId))
                    .ReturnsAsync(student);

            var result = await _userService.GetStudentProfileAsync(studentId, inputDto);

            Assert.NotNull(result);
            Assert.Equal("student_alex", result.Username);

            Assert.Equal(2, result.universalDTO.TotalCount);

            Assert.Single(result.userGames);
            Assert.Equal(95, result.userGames.First().Score);

        }
        [Fact]
        public async Task GetStudentProfileAsync_StudentNotFound_ThrowsNullReferenceException()
        {
            var studentId = Guid.NewGuid();
            var inputDto = new StudentProfileDTO { universalDTO = new UniversalDTO() };

            _userRepositoryMock
                .Setup(r => r.GetStudentWithSessionsAsync(studentId))
                .ReturnsAsync(() => null);

            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _userService.GetStudentProfileAsync(studentId, inputDto));
        }
        [Fact]
        public async Task UpdateStudentProfile_WhenPasswordChanged_UpdatesProfileAndHashesNewPassword() {
            var studentId = Guid.NewGuid();
            var inputDto = new StudentProfileDTO
            {
                Username = "student_dima",
                Email = "student@gmail.com",
                Password = "new_raw_password_123",
                Name = "Dima",
                Surname = "ADawd",
                DateOfBirth = new DateOnly(2005, 4, 8)
            };
            var student = new Student
            {
                Username = "student_dima",
                Email = "student@gmail.com",
                PasswordHash = "raw_password_123",
                Name = "Dima",
                Surname = "ADawd",
                DateOfBirth = new DateOnly(2005, 4, 8)
            };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(studentId)).ReturnsAsync(student);

            _hashServiceMock
        .       Setup(h => h.Verify(inputDto.Password, student.PasswordHash))
                .Returns(false);

            string newHash = "new_hashed_password_xyz";
            _hashServiceMock

                .Setup(h => h.Generate(inputDto.Password))
                .Returns(newHash);
            await _userService.UpdateStudentProfile(studentId, inputDto);

            Assert.Equal(inputDto.Name, student.Name);
            Assert.Equal(inputDto.Surname, student.Surname);
            Assert.Equal(inputDto.Email, student.Email);
            Assert.Equal(newHash, student.PasswordHash);

            _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateStudentProfile_WhenPasswordIsSame_DoesNotRegenerateHash()
        {
            var studentId = Guid.NewGuid();
            var inputDto = new StudentProfileDTO
            {
                Name = "New Dima",
                Password = "same_password"
            };

            var existingStudent = new Student
            {
                Id = studentId,
                PasswordHash = "hashed_same_password"
            };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(studentId)).ReturnsAsync(existingStudent);

            _hashServiceMock
                .Setup(h => h.Verify(inputDto.Password, existingStudent.PasswordHash))
                .Returns(true);
            await _userService.UpdateStudentProfile(studentId, inputDto);

            _userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
        [Fact]
        public async Task UpdateTeacher_WhenPasswordChanged_UpdatesProfileAndHashesNewPassword()
        {
            var teacherId = Guid.NewGuid();
            var inputDto = new TeacherProfileDTO
            {
                Username = "teacher_dima",
                Email = "student@gmail.com",
                Password = "new_raw_password_123",
                Name = "Dima",
                Surname = "ADawd",
                DateOfBirth = new DateOnly(2005, 4, 8),
                WorkPlace = "Kpi",
                Specialization = "ewe"

            };
            var teacher = new Teacher
            {
                Username = "student_dima",
                Email = "student@gmail.com",
                PasswordHash = "raw_password_123",
                Name = "Dima",
                Surname = "ADawd",
                DateOfBirth = new DateOnly(2005, 4, 8),
                WorkPlace = "Kpi",
                Specialization = "ewe"
            };

            _userRepositoryMock.Setup(x => x.GetAllTeacherAsync(teacherId)).ReturnsAsync(teacher);

            _hashServiceMock
                .Setup(h => h.Verify(inputDto.Password, teacher.PasswordHash))
                .Returns(false);

            string newHash = "new_hashed_password_xyz";
            _hashServiceMock

                .Setup(h => h.Generate(inputDto.Password))
                .Returns(newHash);
            await _userService.UpdateTeacherProfile(teacherId, inputDto);

            Assert.Equal(inputDto.Name, teacher.Name);
            Assert.Equal(inputDto.Surname, teacher.Surname);
            Assert.Equal(inputDto.Email, teacher.Email);
            Assert.Equal(newHash, teacher.PasswordHash);
            Assert.Equal(inputDto.WorkPlace, teacher.WorkPlace);
            Assert.Equal(inputDto.Specialization, teacher.Specialization);

            _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateTeacher_WhenPasswordIsSame_DoesNotRegenerateHash()
        {
            var teacherId = Guid.NewGuid();
            var inputDto = new TeacherProfileDTO
            {
                Username = "teacher_dima",
                Email = "teacher@gmail.com",
                Password = "same_password",
                Name = "Dima",
                Surname = "ADawd",
                DateOfBirth = new DateOnly(2005, 4, 8),
                WorkPlace = "KPI",
                Specialization = "History",
                universalDTO = new UniversalDTO()
            };

            var teacher = new Teacher
            {
                Id = teacherId,
                Username = "teacher_dima",
                Email = "teacher@gmail.com",
                PasswordHash = "hashed_same_password",
                Name = "Old Name",
                Surname = "Old Surname",
                DateOfBirth = new DateOnly(2000, 1, 1),
                WorkPlace = "Old Place",
                Specialization = "Old Spec"
            };

            _userRepositoryMock.Setup(x => x.GetAllTeacherAsync(teacherId)).ReturnsAsync(teacher);

            _hashServiceMock
                .Setup(h => h.Verify(inputDto.Password, teacher.PasswordHash))
                .Returns(true);
            await _userService.UpdateTeacherProfile(teacherId, inputDto);

            _userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }


    }

}
