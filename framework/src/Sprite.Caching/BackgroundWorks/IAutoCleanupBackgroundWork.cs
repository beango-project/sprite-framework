using System;

namespace Sprite.Caching
{
    public interface IAutoCleanupBackgroundWork : IBackgroundWork
    {
        TimeSpan ExecutionFrequency { get; set; }
    }
}