namespace WebApplication1
{
    public class MatchSummary
    {
        public string GameName { get; set; }
        public string TagLine { get; set; }
        public string Champion { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public bool Win { get; set; }
        public DateTime GameDate { get; set; }
    }

}
