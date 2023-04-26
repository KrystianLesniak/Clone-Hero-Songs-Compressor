namespace SongsCompressor.Common.Models
{
    public class OverallProgressStatus
    {
        public int TotalEngines { get; set; }
        public int EnginesFinished { get; set; }
        public EngineProgressStatus EngineProgress { get; set; } = new EngineProgressStatus();
        public bool AreAllEnginesComplete() => TotalEngines > 0 && TotalEngines == EnginesFinished;
    }
}
