using QuizFlow.Models.Enums;

namespace QuizFlow.Models
{
    ////если ты создашь объект Quiz и забудешь заполнить Title, там окажется null.
    //Если потом твой бэкенд попытается сделать Title.Length или Title.ToLower(),
    //приложение упадет с ошибкой.
    public abstract class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime AccountCreatedAt { get; set; } = DateTime.UtcNow;
        public UserRole Role { get; protected set; }
        protected User() { }
        protected User(string username, string email, string passwordHash, UserRole role, string name, string surname, DateOnly dateofbirth)
        {
            Id = Guid.NewGuid();
            Username = username;
            Name = name;
            Surname = surname;
            DateOfBirth = dateofbirth;
            PasswordHash = passwordHash;
            Email = email;
            Role = role;
        }

    }
}
