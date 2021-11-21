using System;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Sprite.Threading;

namespace Sprite.Caching
{
    public class AutoCleanupBackgroundWork : IAutoCleanupBackgroundWork, ISupportBindCacheStack
    {
        private bool _isExecute;

        private readonly TimerTask _timer;

        public AutoCleanupBackgroundWork()
        {
            _timer = new TimerTask();
            // _timer.Delay = 0;
            _timer.Elapsed += (_, _) => Cleanup();
        }

        public TimeSpan ExecutionFrequency { get; set; }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _isExecute = true;
            _timer.Period = Convert.ToInt32(ExecutionFrequency.TotalMilliseconds);
            // await Cleanup();
            _timer.Start();
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            _isExecute = false;
            _timer.Stop();
            await Task.CompletedTask;
        }

        public ICacheStack CacheStack { get; private set; }


        public void BindCacheStack(ICacheStack stack)
        {
            Check.NotNull(stack, nameof(stack));
            CacheStack = stack;
        }


        private void Cleanup()
        {
            Console.WriteLine("开始清理数据");

            AsyncContext.Run(() => { CacheStack.CleanUpAsync(); });
        }

        private async Task CleanupAsync()
        {
            // await Task.Delay(ExecutionFrequency).ContinueWith(async _ =>
            // {
            //     // the condition is execute is true, and we need to wait again.
            //     if (_isExecute)
            //     {
            //         Console.WriteLine("开始清理数据");
            //         await _cacheStack.CleanUpAsync();
            //         await Cleanup();
            //     }
            // });
        }
    }
}