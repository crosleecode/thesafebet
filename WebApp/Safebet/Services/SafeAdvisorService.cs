namespace SafeBet.Services
{
    using System.Net.Http;
    using System.Net.Http.Json;
    using SafeBet.Models;

    public class SafeAdvisorService
    {
        private readonly HttpClient _http;
        public SafeAdvisorService(HttpClient http)
        {
            _http = http;
        }
        public async Task<AdviceRequester?> GetAdviceAsync(AdviceRequest req)
        {
            var response = await _http.PostAsJsonAsync("/advise", req);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AdviceRequester>();
        }

    }


}