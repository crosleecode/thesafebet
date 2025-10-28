namespace SafeBet.Services
{

    using System.Net.Http.Json;
    using SafeBet.Models;

    public class SafeAdvisorService
    {
        private readonly HttpClient _http;
        public SafeAdvisorService(HttpClient http)
        {
            _http = http;
        }


    }
}