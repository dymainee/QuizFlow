using QuizFlow.Models.Enums;

namespace QuizFlow.DTO
{
    public class UniversalDTO
    {
        public SortOrder sortOrder { get; set;}
        public string sortField { get; set;}

        //pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public int TotalCount { get; set; } //общее количество записей
    }
}
