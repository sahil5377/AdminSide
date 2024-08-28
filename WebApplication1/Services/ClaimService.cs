using InsuranceApi.DTOs;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebApplication1.Services
{
    public class ClaimService : IClaimService
    {
        private readonly HttpClient _httpClient;

        public ClaimService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ClaimDto>> GetAllClaimsAsync()
        {
            string endpoint = "Claim";
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var claims = JsonSerializer.Deserialize<List<ClaimDto>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return claims;
        }

        public async Task UpdateClaimStatusAsync(int claimId, string status, decimal dispenseAmount)
        {
            var updateDto = new { Status = status, DispenseAmount = dispenseAmount };
            var response = await _httpClient.PutAsJsonAsync($"Claim/{claimId}/status", updateDto);
            response.EnsureSuccessStatusCode();
        }
        public async Task<int> GetClaimCountAsync()
        {
            var response = await _httpClient.GetAsync("Claim/count");
            response.EnsureSuccessStatusCode();
            var count = await response.Content.ReadFromJsonAsync<int>();
            return count;

        }

    }

    public interface IClaimService
    {
        Task<List<ClaimDto>> GetAllClaimsAsync();
        Task<int> GetClaimCountAsync();
        Task UpdateClaimStatusAsync(int claimId, string status, decimal dispenseAmount);
    }
}