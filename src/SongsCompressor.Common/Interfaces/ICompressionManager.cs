using SongsCompressor.Common.Enums;
using SongsCompressor.Common.Models;

namespace SongsCompressor.Common.Interfaces
{
    public interface ICompressionManager
    {
        Task Initialize(IList<string> dictionaries, IList<OptionsEnum> options);
        Task Start();
        Task<OverallProgressStatus> GetCurrentProgressStatus();
    }
}
