using System;

namespace Sprite.UidGenerator.Providers.Snowflake
{
    public class Snowflake : SnowflakeDrift
    {
        public Snowflake(SnowflakeGeneratorOptions options) : base(options)
        {
        }

        public override long NextId()
        {
            lock (_SyncLock)
            {
                long currentTimeTick = GetCurrentTimeTick();

                if (_LastTimeTick == currentTimeTick)
                {
                    if (_CurrentSeqNumber++ > MaxSeqNumber)
                    {
                        _CurrentSeqNumber = MinSeqNumber;
                        currentTimeTick = GetNextTimeTick();
                    }
                }
                else
                {
                    _CurrentSeqNumber = MinSeqNumber;
                }

                if (currentTimeTick < _LastTimeTick)
                {
                    throw new Exception($"Time error for {_LastTimeTick - currentTimeTick} milliseconds");
                }

                _LastTimeTick = currentTimeTick;
                var result = ((currentTimeTick << _TimestampShift) + ((long)WorkerId << SeqBitLength) + (uint)_CurrentSeqNumber);

                return result;
            }
        }
    }
}