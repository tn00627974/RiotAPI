namespace LolTeamTracker.Controllers
{
    public class ChampionRoot
    {
        public Dictionary<string, ChampionData> Data { get; set; }
    }

    public class ChampionData
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        // 其它欄位可自行忽略
    }
}
