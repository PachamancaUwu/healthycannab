using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;


namespace healthycannab.Services
{
    public class EmailValidation
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public EmailValidation(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ZeroBounce:ApiKey"];
        }

        public async Task<ZeroBounceResponse> ValidateEmailAsync(string email)
        {
            var requestUrl = $"https://api.zerobounce.net/v2/validate?api_key={_apiKey}&email={email}";
            var response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Error en la solicitud a ZeroBounce.");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ZeroBounceResponse>(content);
        }
    }

    public class ZeroBounceResponse
    {
        public string Address { get; set; }
        public string Status { get; set; } // "valid", "invalid", "catch-all", etc.
        public string SubStatus { get; set; } // Explicaciones adicionales de estado, como "mailbox_not_found"
        public string Account { get; set; }
        public string Domain { get; set; }
        public string Country { get; set; }
    }
}