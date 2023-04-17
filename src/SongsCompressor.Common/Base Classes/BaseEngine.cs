using SongsCompressor.Common.Interfaces;
using SongsCompressor.Common.Models;

namespace SongsCompressor.Common.Base_Classes
{
    public abstract class BaseEngine : ICompressionEngine
    {
        public virtual int ExecutionOrder => 100;

        public bool Completed { get; set; }

        public Task Complete()
        {
            Completed = true;
            return Task.CompletedTask;
        }

        public abstract Task<EngineProgressStatus> GetCurrentProgress();

        public abstract Task Start();


    }
}
