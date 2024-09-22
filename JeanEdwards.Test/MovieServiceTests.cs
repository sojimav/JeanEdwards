using JeanEdwardTask.API.DataTransfer;
using JeanEdwardTask.API.Integrations;
using JeanEdwardTask.API.Services;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;

namespace JeanEdwardTask.Test
{
    public class MovieServiceTests
    {
        private readonly ImdbClientIntegration _movieService;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly OMDb _oMDb;

        public MovieServiceTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://www.omdbapi.com/")
            };

            
            _oMDb = new OMDb
            {
                Key = "abf7f2d", 
                BaseUrl = "http://www.omdbapi.com/"
            };

            // Create an IOptions<OMDb> instance
            var options = Options.Create(_oMDb);

            // Initialize ImdbClientIntegration with IOptions<OMDb>
            _movieService = new ImdbClientIntegration(httpClient, options);
        }



        [Fact]
        public async Task SearchMoviesAsync_ShouldReturnMovies_WhenMoviesAreFound()
        {
            // Arrange
            var searchQuery = "Inception";
            var movieSearchResult = new MovieSearchResult
            {
                TotalResults = 1,
                Search = new List<Movie>
                {
                    new Movie { Title = "Inception", Year = "2010", ImdbID = "tt1375666" }
                }
            };
            var jsonResponse = JsonConvert.SerializeObject(movieSearchResult);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse),
                });

            // Act
            var result = await _movieService.SearchMoviesAsync(search: searchQuery);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Search);
            Assert.Equal("Inception", result.Search[0].Title);
        }

        [Fact]
        public async Task SearchMoviesAsync_ShouldReturnEmpty_WhenNoMoviesAreFound()
        {
            // Arrange
            var searchQuery = "UnknownMovie";
            var movieSearchResult = new MovieSearchResult { TotalResults = 0, Search = new List<Movie>() };
            var jsonResponse = JsonConvert.SerializeObject(movieSearchResult);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse),
                });

            // Act
            var result = await _movieService.SearchMoviesAsync(search: searchQuery);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Search);
        }

        [Fact]
        public async Task GetMovieDetailsAsync_ShouldReturnMovieDetails_WhenValidImdbIdIsProvided()
        {
            // Arrange
            var imdbId = "tt1375666"; // Example IMDb ID
            var movieResponse = new MovieResponse
            {
                Title = "Inception",
                Year = "2010",
                imdbID = imdbId,
                imdbRating = "8.8"
            };
            var jsonResponse = JsonConvert.SerializeObject(movieResponse);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse),
                });

            // Act
            var result = await _movieService.GetMovieDetailsAsync(imdbId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Inception", result.Title);
            Assert.Equal("2010", result.Year);
            Assert.Equal("8.8", result.imdbRating);
        }

        [Fact]
        public async Task GetMovieDetailsAsync_ShouldReturnEmptyResponse_WhenInvalidImdbIdIsProvided()
        {
            // Arrange
            var imdbId = "invalid"; // Invalid IMDb ID
            var movieResponse = new MovieResponse(); // Return an empty response for invalid IMDb ID
            var jsonResponse = JsonConvert.SerializeObject(movieResponse);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse),
                });

            // Act
            var result = await _movieService.GetMovieDetailsAsync(imdbId);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Title); // Invalid movie returns no title
        }
    }

}
