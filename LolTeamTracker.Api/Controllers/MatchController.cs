using LolTeamTracker.Api.Models;
using LolTeamTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LolTeamTracker.Api.Controllers;

[ApiController]
//[Route("[controller]")]
[Route("api/match")]
public class MatchController : ControllerBase
{
    private readonly RiotApiService _riot;

    public MatchController (RiotApiService riot)
    {
        _riot = riot;
    }

    /// <summary>
    /// 取得指定玩家的比賽列表
    /// </summary>
    /// <remarks>
    /// 範例：
    /// GET /api/match/Faker/TW2
    /// </remarks>
    /// <param name="gameName">召喚師名稱，例如Faker</param>
    /// <param name="tagLine">地區代碼，例如 TW2</param>
    /// <param name="count">搜尋場次</param>
    /// <returns></returns>
    [HttpGet("{gameName}/{tagLine}")]
    public async Task<IActionResult> GetMatchList(string gameName, string tagLine, int count = 50)
    {
        // 預設0~100不得超過API限制
        //if (count <= 0) { count = 50; }
        //else if (count > 100) { count = 100; }
        count = Math.Clamp(count, 50, 100);

        var puuid = await _riot.GetPuuidAsync(gameName, tagLine); 
        var matchIds = await _riot.GetMatchIdsAsync(puuid,0,count);
        var result = new List<MatchSummary>();

        foreach (var matchId in matchIds)
        {
            try
            {
                var summary = await _riot.GetMatchSummaryAsync(matchId, puuid, gameName, tagLine);
                if (summary != null)
                    result.Add(summary);
            }
            catch (Exception ex)
            {
                return BadRequest($"讀取比賽 {matchId} 失敗：{ex.Message}");
                    
            }
        }
        return this.Ok(new
        {
            count = result.Count,
            data = result
        });
    }


}
