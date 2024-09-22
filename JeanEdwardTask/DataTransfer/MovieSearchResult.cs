namespace JeanEdwardTask.API.DataTransfer
{
    public class MovieSearchResult
    {
        public List<Movie> Search { get; set; }
        public int TotalResults { get; set; }
        public string Response { get; set; }
    }
}


