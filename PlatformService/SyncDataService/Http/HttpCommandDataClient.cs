using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace SyncDataService.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task SendPlatformToCommand(PlatformReadDto platformReadDto)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(platformReadDto),
                Encoding.UTF8,
                MediaTypeNames.Application.Json

            );

            var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}/platforms", httpContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> Sync POST to Command Service was OK!");
            }
            else
            {
                Console.WriteLine("--> Sync POST to Command Service was NOT OK!");
            }
        }
    }
}