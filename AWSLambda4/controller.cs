using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public MovieController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromBody] object movieData)
        {
            var lambdaUrl = "https://w9nbvf6p6e.execute-api.us-east-1.amazonaws.com/v1/create-movie";
            var response = await _httpClient.PostAsJsonAsync(lambdaUrl, movieData);

            if (response.IsSuccessStatusCode)
            {
                // Optionally, process the response from the Lambda function
                return (IActionResult)Ok(await response.Content.ReadAsStringAsync());
            }
            else
            {
                // Handle errors or unsuccessful responses
                return (IActionResult)StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
        }
    }
}
