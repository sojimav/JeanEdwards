namespace JeanEdwardTask.API.DataTransfer
{
    public class MovieQuery
    {
        public string? Search { get; set; } = string.Empty;
        public string? Title { get; set; } = string.Empty;
        public string? Year { get; set; } = string.Empty;
        public string? Plot { get; set; } = "full";
    }
}
