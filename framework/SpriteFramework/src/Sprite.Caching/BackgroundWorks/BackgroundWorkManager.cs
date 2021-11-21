using System;
using System.Linq;
using System.Threading.Tasks;
using Sprite.Caching.BackgroundWorks;

namespace Sprite.Caching
{
    internal class BackgroundWorkManager : IBackgroundWorkManager, IAsyncDisposable
    {
        private bool _isDispose;

        public BackgroundWorkManager(IBackgroundWork[] works)
        {
            AllWorks = Array.Empty<IBackgroundWork>();
            if (works != null && works.Length > 0)
            {
                AllWorks = works;
            }
        }

        private IBackgroundWork[] AllWorks { get; }

        public async ValueTask DisposeAsync()
        {
            if (_isDispose)
            {
                return;
            }

            foreach (var work in AllWorks)
            {
                if (work is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                else if (work is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
            }

            _isDispose = true;
        }


        public void BindingCacheStack(ICacheStack cacheStack)
        {
            foreach (var work in AllWorks)
            {
                if (work is ISupportBindCacheStack bindCacheStackWork)
                {
                    bindCacheStackWork.BindCacheStack(cacheStack);
                }
            }
        }

        public async Task Run()
        {
            foreach (var work in AllWorks)
            {
                await work.StartAsync();
            }
        }

        public async Task Run(IBackgroundWork backgroundWork)
        {
            var works = AllWorks.Where(x => x == backgroundWork);
            foreach (var wk in works)
            {
                await wk.StartAsync();
            }
        }
    }
}