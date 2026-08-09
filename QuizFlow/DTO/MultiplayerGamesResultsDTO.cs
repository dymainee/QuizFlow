namespace QuizFlow.DTO
{
    public class MultiplayerGamesResultsDTO
    {
        public List<UserQuizSessionDTO> userGames { get; set; } = new List<UserQuizSessionDTO>();
        public UniversalDTO universalDTO { get; set; } = new UniversalDTO();
        public string? title_filter { get; set; }
    }
}
