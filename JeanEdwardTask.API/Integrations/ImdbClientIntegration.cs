using JeanEdwardTask.API.DataTransfer;
using JeanEdwardTask.API.Services;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace JeanEdwardTask.API.Integrations
{
    public class ImdbClientIntegration
    {
        private readonly HttpClient _client;

        //public static readonly string apiKey = "abf7f2d";
        //private static readonly string baseUrl = "";

        public ImdbClientIntegration(HttpClient httpClient)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            _client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _client.DefaultRequestHeaders.Add("User-Agent", "Custom User Agent");
            _client.Timeout = TimeSpan.FromMinutes(30);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            
        }



        public async Task<T> GetAsync<T>(string relativePath)
        {
            Uri requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture, relativePath));

            var request = new HttpRequestMessage() { RequestUri = requestUrl, Method = HttpMethod.Get };

            var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            var data = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Response:: " + data);

            return JsonConvert.DeserializeObject<T>(data);
        }

        public async Task<T> GetAsync<T>(string relativePath, object content)
        {
            Uri requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture, relativePath));

            var request = new HttpRequestMessage() { RequestUri = requestUrl, Method = HttpMethod.Get, Content = CreateHttpContent(content) };

            var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            var data = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Response:: " + data);

            return JsonConvert.DeserializeObject<T>(data);
        }

        private Uri CreateRequestUri(string relativePath, string queryString = "")
        {
            var endpoint = new Uri(string.Concat(OMDb.BaseUrl, relativePath));
            var uriBuilder = new UriBuilder(endpoint);
            if (!string.IsNullOrEmpty(queryString))
            {
                uriBuilder.Query = queryString;
            }
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Common method for making POST calls
        /// </summary>
        public async Task<T> PostAsync<T>(string relativePath, object content)
        {
            Uri requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture, relativePath));

            var request = new HttpRequestMessage() { RequestUri = requestUrl, Method = HttpMethod.Post, Content = CreateHttpContent(content) };

            var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            var data = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Response:: " + data);

            return JsonConvert.DeserializeObject<T>(data);
        }

        private HttpContent CreateHttpContent(object content)
        {
            var json = JsonConvert.SerializeObject(content, MicrosoftDateFormatSettings);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private static JsonSerializerSettings MicrosoftDateFormatSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                };
            }
        }


        public async Task<MovieSearchResult> SearchMoviesAsync(string? search = "", string? title = "", string? year = "", string? plot = "full")
        {
            var url = $"/?apikey={OMDb.Key}&s={search}&t={title}&y={year}&plot={plot}";
            var response = await GetAsync<MovieSearchResult>(url);
            return  response ?? new MovieSearchResult();
            
        }

        public async Task<MovieResponse> GetMovieDetailsAsync(string imdbId)
        {
            var url = $"/?apikey={OMDb.Key}&i={imdbId}&plot=full";
            var response = await GetAsync<MovieResponse>(url);
            return response ?? new MovieResponse();
        }


    }
}


