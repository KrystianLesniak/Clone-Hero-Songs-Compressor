using SongsCompressor.Common.Enums;
using SongsCompressor.Common.Models;

namespace SongsCompressor.Common.Interfaces
{
    public interface ICompressionManager
    {
        Task Initialize(UserSettings settings);
        Task Start();
        Task<OverallProgressStatus> GetCurrentProgressStatus();
    }
}
