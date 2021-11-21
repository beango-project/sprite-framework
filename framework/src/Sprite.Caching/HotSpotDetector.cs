using System.Threading.Tasks;

namespace Sprite.Caching
{
    public class HotSpotDetector
    {
        private readonly CacheHeatHolder _heatHolder;

        public HotSpotDetector(CacheHeatHolder heatHolder)
        {
            _heatHolder = heatHolder;
        }

        public ICacheLayer CacheLayer { get; }

        /// <summary>
        /// 热度阈值
        /// </summary>
        public int Threshold { get; set; }

        public string[] Hots { get; private set; }


        private void AutoScanTask()
        {
            Hots = _heatHolder.GetTopHotKey(10);
        }

        public async Task Run()
        {
            await Task.Run(AutoScanTask).ConfigureAwait(false);
        }
    }
}