namespace QuizFlow.Application.Interfaces
{
    public interface IHashService
    {
        public string Generate(string password);
        public bool Verify(string password, string hashedPassword);
    }
}
