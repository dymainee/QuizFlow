using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using QuizFlow.Data;
using QuizFlow.Infrastructure.Repositories;
using QuizFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizFlowTests.RepositoriesTests
{
    public class UserRepositoryTests
    {
        private ApplicationContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationContext(options);
        }
        [Fact]
        public async Task CreateAsync_ValidStudent_AddsUserToDatabase()
        {
            using var context = GetInMemoryContext();
            var repo = new UserRepository(context);
            Guid userId = Guid.NewGuid();

            var student = new Student
            {
                Id = userId,
                Name = "Dima",
                Surname = "ADawd",
                Username = "eweqw",
                DateOfBirth = new DateOnly(2005, 4, 8), // Используем DateTime вместо string
                Email = "weqewq@gmail.com",
                PasswordHash = "eqweq",
                AccountCreatedAt = DateTime.UtcNow,
                QuizSessions = new List<QuizSession>()
            };

            await repo.CreateAsync(student);
            await repo.SaveChangesAsync();

            var result = await context.Users.FindAsync(userId) as Student;

            Assert.NotNull(result);
            Assert.Single(context.Users);
            Assert.Equal(userId, result.Id);
        }
        [Fact]
        public async Task CreateAsync_ValidTeacher_AddsTeacherToDatabase()
        {
            using var context = GetInMemoryContext();
            var repo = new UserRepository(context);
            Guid teacherId = Guid.NewGuid();

            var teacher = new Teacher
            {
                Id = teacherId,
                Name = "Dima",
                Surname = "ADawd",
                Username = "teacher_dima",
                DateOfBirth = new DateOnly(1990, 5, 15),
                Email = "teacher@gmail.com",
                PasswordHash = "eqweq",
                AccountCreatedAt = DateTime.UtcNow,
                WorkPlace = "School #1",             
                Specialization = "Mathematics",     
                Quizzes = new List<Quiz>()
            };

            // 2. ACT
            await repo.CreateAsync(teacher);
            await repo.SaveChangesAsync();

            var result = await context.Users.FindAsync(teacherId) as Teacher;

            Assert.Single(context.Users);
            Assert.Equal(teacherId, result.Id);
            Assert.Equal("School #1", result.WorkPlace); 
        }

        [Fact]
        public async Task GetByEmailAsync_GetsAnEmail_User() {
            var context = GetInMemoryContext();
            var repot = new UserRepository(context);
            Guid teacherId = Guid.NewGuid();
            string email = "teacher@gmail.com";
            var teacher = new Teacher
            {
                Id = teacherId,
                Name = "Dima",
                Surname = "ADawd",
                Username = "teacher_dima",
                DateOfBirth = new DateOnly(1990, 5, 15),
                Email = email,
                PasswordHash = "eqweq",
                AccountCreatedAt = DateTime.UtcNow,
                WorkPlace = "School #1",
                Specialization = "Mathematics",
                Quizzes = new List<Quiz>()
            };

            await context.AddAsync(teacher);
            await context.SaveChangesAsync();

            var result = await repot.GetByEmailAsync(email);

            Assert.NotNull(email);
            Assert.Equal(teacherId, result.Id);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task GetAllTeacherAsync_GetsAnId_TeacherEntity() {
            var context = GetInMemoryContext();
            var repo = new UserRepository(context);
            Guid teacherId = Guid.NewGuid();
            string email = "teacher@gmail.com";
            var teacher = new Teacher
            {
                Id = teacherId,
                Name = "Dima",
                Surname = "ADawd",
                Username = "teacher_dima",
                DateOfBirth = new DateOnly(1990, 5, 15),
                Email = email,
                PasswordHash = "eqweq",
                AccountCreatedAt = DateTime.UtcNow,
                WorkPlace = "School #1",
                Specialization = "Mathematics",
                Quizzes = new List<Quiz>()
            };

            await context.AddAsync(teacher);
            await context.SaveChangesAsync();

            var result = await repo.GetAllTeacherAsync(teacherId);
            Assert.NotNull(teacherId);
            Assert.Equal(teacherId, result.Id);
            Assert.IsType<Teacher>(result);
        }
        [Fact]
        public async Task GetStudentWithSessionsAsync_WhenStudentExists_ReturnsStudentWithSessionsAndQuiz()
        {
            var context = GetInMemoryContext();
            var repo = new UserRepository(context);
            var studentId = Guid.NewGuid();
            var sessionid = Guid.NewGuid();
            var quiz = new Quiz
            {
                Id = Guid.NewGuid(),
                Title = "Geography",
                Questions = new List<Question>()
            };
            QuizSession quizSession = new QuizSession
            {
                Id = sessionid,
                Quiz = quiz,
                UserAnswers = new List<UserAnswer>()

            };
            var student = new Student
            {
                Id = studentId,
                Name = "Dima",
                Surname = "ADawd",
                Username = "eweqw",
                DateOfBirth = new DateOnly(2005, 4, 8), // Используем DateTime вместо string
                Email = "weqewq@gmail.com",
                PasswordHash = "eqweq",
                AccountCreatedAt = DateTime.UtcNow,
                QuizSessions = new List<QuizSession>() {quizSession }
            };
            await context.AddAsync(student);
            await repo.SaveChangesAsync();

            var result = await repo.GetStudentWithSessionsAsync(studentId);

            Assert.NotNull(result);
            Assert.Equal(studentId, result.Id);
            Assert.IsType<Student>(result);

            Assert.NotNull(result.QuizSessions);
            Assert.Single(result.QuizSessions);

            var session = result.QuizSessions.First();
            Assert.Equal(sessionid, session.Id);
            Assert.NotNull(session.Quiz);
            Assert.Equal("Geography", session.Quiz.Title);


        }

    }
}
