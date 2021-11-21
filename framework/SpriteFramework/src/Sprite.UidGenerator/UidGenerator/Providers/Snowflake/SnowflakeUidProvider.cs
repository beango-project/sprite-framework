using System;
using System.Threading;

namespace Sprite.UidGenerator.Providers.Snowflake
{
    /// <summary>
    /// 雪花算法Uid提供者
    /// </summary>
    public class SnowflakeUidProvider : ISnowflakeUidProvider
    {
        private readonly SnowflakeGeneratorOptions _options;

        private readonly ISnowflake _snowflakeWorker;

        public SnowflakeUidProvider(SnowflakeGeneratorOptions options)
        {
            if (options == null)
            {
                throw new ApplicationException("options error.");
            }

            // 1.BaseTime
            if (options.BaseTime < DateTime.Now.AddYears(-50) || options.BaseTime > DateTime.Now)
            {
                throw new ApplicationException("BaseTime error.");
            }

            // 2.WorkerIdBitLength
            int maxLength = options.TimestampType == 0 ? 22 : 31; // （秒级时间戳时放大到31位）
            if (options.WorkerIdBitLength <= 0)
            {
                throw new ApplicationException("WorkerIdBitLength error.(range:[1, 21])");
            }

            if (options.DataCenterIdBitLength + options.WorkerIdBitLength + options.SeqBitLength > maxLength)
            {
                throw new ApplicationException("error：DataCenterIdBitLength + WorkerIdBitLength + SeqBitLength <= " + maxLength);
            }

            // 3.WorkerId & DataCenterId
            var maxWorkerIdNumber = (1 << options.WorkerIdBitLength) - 1;
            if (maxWorkerIdNumber == 0)
            {
                maxWorkerIdNumber = 63;
            }

            if (options.WorkerId < 0 || options.WorkerId > maxWorkerIdNumber)
            {
                throw new ApplicationException("WorkerId error. (range:[0, " + maxWorkerIdNumber + "]");
            }

            var maxDataCenterIdNumber = (1 << options.DataCenterIdBitLength) - 1;
            if (options.DataCenterId < 0 || options.DataCenterId > maxDataCenterIdNumber)
            {
                throw new ApplicationException("DataCenterId error. (range:[0, " + maxDataCenterIdNumber + "]");
            }

            // 4.SeqBitLength
            if (options.SeqBitLength < 2 || options.SeqBitLength > 21)
            {
                throw new ApplicationException("SeqBitLength error. (range:[2, 21])");
            }

            // 5.MaxSeqNumber
            var maxSeqNumber = (1 << options.SeqBitLength) - 1;
            if (maxSeqNumber == 0)
            {
                maxSeqNumber = 63;
            }

            if (options.MaxSeqNumber < 0 || options.MaxSeqNumber > maxSeqNumber)
            {
                throw new ApplicationException("MaxSeqNumber error. (range:[1, " + maxSeqNumber + "]");
            }

            // 6.MinSeqNumber
            if (options.MinSeqNumber < 5 || options.MinSeqNumber > maxSeqNumber)
            {
                throw new ApplicationException("MinSeqNumber error. (range:[5, " + maxSeqNumber + "]");
            }

            switch (options.Type)
            {
                case SnowflakeType.Drift:
                    _snowflakeWorker = new SnowflakeDrift(options);
                    break;
                default:
                    if (options.DataCenterIdBitLength == 0 && options.TimestampType == 0)
                    {
                        _snowflakeWorker = new Snowflake(options);
                    }
                    else
                    {
                        _snowflakeWorker = new SnowflakeWithDataCenter(options);
                    }

                    break;
            }

            if (options.Type == SnowflakeType.Drift)
            {
                Thread.Sleep(500);
            }

            _options = options;
        }

        public long NextId()
        {
            return _snowflakeWorker.NextId();
        }

        public virtual long Create()
        {
            return _snowflakeWorker.NextId();
        }
    }
}