using JeanEdwardTask.API.DataTransfer;
using JeanEdwardTask.API.Integrations;
using JeanEdwardTask.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace JeanEdwardTask.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly ImdbClientIntegration _imdbClient;
        private readonly CacheLatest _cachelatest;

        public MoviesController(ImdbClientIntegration imdbClient, CacheLatest cache)
        {
            _imdbClient = imdbClient;
            _cachelatest = cache;
        }

        // Endpoint to search movies by title
        [HttpGet("search")]
        public async Task<IActionResult> SearchMovies([FromQuery] MovieQuery query)
        {
            _cachelatest.SaveSearchQuery(query.Search!);
            var result = await _imdbClient.SearchMoviesAsync(query.Search, query.Title, query.Year, query.Plot);
            if (result.Response == "True")
            {
                return Ok(result);
            }
            return NotFound(new { Message = "Movie not found" });
        }

        // Endpoint to get movie details by IMDb ID
        [HttpGet("details/{imdbId}")]
        public async Task<IActionResult> GetMovieDetails([FromRoute]string imdbId)
        {
            var movie = await _imdbClient.GetMovieDetailsAsync(imdbId);
            return movie != null ? Ok(movie) : NotFound(new { Message = "Movie not found" });
        }


        // Get latest searches
        [HttpGet("latest-searches")]
        public IActionResult GetLatestSearches()
        {
            var response  = _cachelatest.GetLatestSearches();
            return Ok(response);
        }
    }
}
