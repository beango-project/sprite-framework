using System;
using System.Threading;
using System.Threading.Tasks;
using Nito.Disposables;

namespace Sprite.Threading
{
    public class TimerTask : IDisposable, IAsyncDisposable
    {
        private readonly Timer _timer;

        private volatile bool _executing;

        private volatile bool _isRunning;


        public TimerTask()
        {
            _timer = new Timer(TimerCallBack, null, Timeout.Infinite, Timeout.Infinite);
        }

        public TimerTask(int period, int? delay) : this()
        {
            Period = period;
            Delay = delay;
        }

        public int? Delay { get; set; }

        /// <summary>
        /// 忽略异常
        /// 当值为true时在执行过程中不会因异常而中断
        /// 可以通过设置<see cref="ExceptionNotifier" />来获取异常
        /// </summary>
        public bool IgnoreException { get; set; }

        /// <summary>
        /// 执行周期
        /// </summary>
        public int Period { get; set; }

        public ValueTask DisposeAsync()
        {
            return this.ToAsyncDisposable().DisposeAsync();
        }

        public void Dispose()
        {
            _timer?.Dispose();
            GC.SuppressFinalize(this);
        }


        public event EventHandler Elapsed;


        // public int RepeatCount { get; set; }

        /// <summary>
        /// 异常通知者
        /// <example>
        /// ExceptionNotifier+=(o,ex)=>{
        /// Console.WriteLine(ex.Message);
        /// }
        /// </example>
        /// </summary>
        public event EventHandler<Exception> ExceptionNotifier;


        public void Start()
        {
            lock (_timer)
            {
                //当DueTime为0时代表着Timer会在开始时立刻执行Elapsed(CallBack)里的事件
                _timer.Change(Delay ?? Period, Timeout.Infinite);
                _isRunning = true;
            }
        }

        public void Stop()
        {
            lock (_timer)
            {
                _timer.Change(-1, Timeout.Infinite);
                while (_executing)
                {
                    Monitor.Wait(_timer);
                }

                _isRunning = false;
            }
        }

        private void TimerCallBack(object state)
        {
            lock (_timer)
            {
                if (!_isRunning)
                {
                    return;
                }

                //启动计时器
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                _executing = true;
            }

            try
            {
                Elapsed?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                if (!IgnoreException)
                {
                    throw new Exception(e.Message);
                }

                ExceptionNotifier?.Invoke(this, e);
            }
            finally
            {
                lock (_timer)
                {
                    _executing = false;
                    if (_isRunning)
                    {
                        _timer.Change(Period, Timeout.Infinite);
                    }

                    Monitor.Pulse(_timer);
                }
            }
        }
    }
}