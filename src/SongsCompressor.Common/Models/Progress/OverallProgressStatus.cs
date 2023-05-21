namespace SongsCompressor.Common.Models
{
    public class OverallProgressStatus
    {
        public int TotalEngines { get; set; }
        public int EnginesFinished { get; set; }
        public EngineProgressStatus EngineProgress { get; set; } = new EngineProgressStatus();
        public int OverallEnginePercentageComplete { get; private set; }
        public bool AllEnginesComplete => TotalEngines == EnginesFinished;

        public void UpdateProgress(EngineProgressStatus currentlyRunningEngineProgress)
        {
            EngineProgress = currentlyRunningEngineProgress;

            if (AllEnginesComplete)
            {
                OverallEnginePercentageComplete = 100;
                return;
            }

            var enginesPercentage = (EnginesFinished * 100 / TotalEngines);
            var enginePercentage = (int)(EngineProgress.PercentageComplete * (1 / (double)TotalEngines));

            OverallEnginePercentageComplete = enginesPercentage + enginePercentage;
        }
    }
}
