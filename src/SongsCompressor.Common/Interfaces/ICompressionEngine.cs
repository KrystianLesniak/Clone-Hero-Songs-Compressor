using SongsCompressor.Common.Models;

namespace SongsCompressor.Common.Interfaces
{
    public interface ICompressionEngine
    {
        Task Start();
        Task<EngineProgressStatus> GetCurrentProgress();
        int ExecutionOrder { get; }
    }
}
