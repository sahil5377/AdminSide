using InsuranceApi.DTOs;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebApplication1.Services
{
    public class HospitalService : IHospitalService
    {
        private readonly HttpClient _httpClient;

        public HospitalService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<HospitalDto>> GetAllHospitalsAsync()
        {
            var response = await _httpClient.GetAsync("Hospital");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var hospitals = JsonSerializer.Deserialize<List<HospitalDto>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return hospitals;
        }

        public async Task AddHospitalAsync(HospitalDto hospital)
        {
            var response = await _httpClient.PostAsJsonAsync("Hospital", hospital);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateHospitalAsync(HospitalDto hospital)
        {
            var response = await _httpClient.PutAsJsonAsync($"Hospital/{hospital.HospitalId}", hospital);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteHospitalAsync(int hospitalId)
        {
            var response = await _httpClient.DeleteAsync($"Hospital/{hospitalId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<int> GetHospitalCountAsync()
        {
            var response = await _httpClient.GetAsync("Hospital/count");
            response.EnsureSuccessStatusCode();
            var count = await response.Content.ReadFromJsonAsync<int>();
            return count;
        }

        public async Task<List<HospitalDto>> SearchHospitalsAsync(string searchTerm)
        {
            string endpoint = $"Hospital/search?term={searchTerm}";
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var hospitals = JsonSerializer.Deserialize<List<HospitalDto>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return hospitals;
        }
    }

    public interface IHospitalService
    {
        Task<List<HospitalDto>> GetAllHospitalsAsync();
        Task AddHospitalAsync(HospitalDto hospital);
        Task UpdateHospitalAsync(HospitalDto hospital);
        Task DeleteHospitalAsync(int hospitalId);
        Task<int> GetHospitalCountAsync();
        Task<List<HospitalDto>> SearchHospitalsAsync(string searchTerm);
    }
}