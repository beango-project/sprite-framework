using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Sprite.Caching
{
    public class CacheLayerListener : ICacheLayerListener, INotifyPropertyChanged
    {
        private HotSpotDetector _detector;
        public int HitStatistics { get; }
        public CacheHeatHolder CacheHeatHolder { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        // public string[] Hots
        // {
        //     get
        //     {
        //       OnPropertyChanged(nameof(Hots));  
        //     };
        //
        // };

        public void Test()
        {
            _detector.Run();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}