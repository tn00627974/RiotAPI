using LolTeamTracker.Api.Models;
using LolTeamTracker.Api.Services;
using Microsoft.AspNetCore.Hosting;

namespace LolTeamTracker.Api.Services
{
    public class MatchAnalyzer
    {
        private readonly RiotApiService _riotApiService;
        private readonly IWebHostEnvironment _env;

        public MatchAnalyzer(RiotApiService riotApiService, IWebHostEnvironment env)
        {
            _riotApiService = riotApiService;
            _env = env;
        }

        public async Task RunAsync()
        {
            
        }

        private async Task LoadTeamFromJson(string fileName)
        {
            string savePath = Path.Combine(_env.ContentRootPath, "Data", "Static", fileName);
            if (!File.Exists(savePath))
            {
                throw new FileNotFoundException($"檔案 {fileName} 不存在於 {savePath}");
            }
            string json = await File.ReadAllTextAsync(savePath);
            var team = JsonSerializer.Deserialize<List<PlayerInfo>>(json);
            return team ?? new List<PlayerInfo>();
        }

    }
}
