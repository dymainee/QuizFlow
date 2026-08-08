namespace QuizFlow.Api
{
    public class QrGenerateRequest
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;

        //optional 
        public int Size { get; set; } = 300;
        public string Format { get; set; } = "png";
        public string Color { get; set; } = "#000000";
        public string BgColor { get; set; } = "#ffffff";
    }
}
