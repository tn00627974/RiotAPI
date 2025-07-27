using LolTeamTracker.Api.Models;
using LolTeamTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LolTeamTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchController : ControllerBase
{
    private readonly RiotApiService _riot;
    private readonly MatchAnalyzer _analyzer;


    public MatchController (RiotApiService riot,MatchAnalyzer analyzer)
    {
        _riot = riot;
        _analyzer = analyzer;
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
    /// <summary>
    /// 測試呼叫 RunAsync()，進行一次分析流程
    /// </summary>
    [HttpGet("test-run")]
    public async Task<IActionResult> TestRunAsync()
    {
        try
        {
            await _analyzer.RunAsync();
            return Ok("分析完成");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"錯誤：{ex.Message}");
        }
    }




}
