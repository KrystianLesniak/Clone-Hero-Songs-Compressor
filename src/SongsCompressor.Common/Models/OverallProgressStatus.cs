using SongsCompressor.Common.Enums;

namespace SongsCompressor.Common.Models
{
    public class OverallProgressStatus
    {
        public int OverallPercentageComplete { get; set; }
        public EngineProgressStatus EngineProgress { get; set; } = new EngineProgressStatus();

        public ProgressStatusEnum Status = 0;

        public bool IsComplete() => Status == ProgressStatusEnum.Complete;
    }
}
