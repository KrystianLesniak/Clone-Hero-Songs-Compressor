using SongsCompressor.Common.Models;

namespace SongsCompressor.Common.Interfaces
{
    public interface ICompressionEngine
    {
        Task Start();
        Task Complete();
        Task<EngineProgressStatus> GetCurrentProgress();
        int ExecutionOrder { get; }
        bool Completed { get; set; }
    }
}
