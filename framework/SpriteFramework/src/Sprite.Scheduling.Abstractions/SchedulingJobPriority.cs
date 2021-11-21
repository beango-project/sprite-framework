namespace Sprite.Scheduling.Abstractions
{
    public enum SchedulingJobPriority
    {
        /// <summary>
        /// 低
        /// </summary>
        Low,

        /// <summary>
        /// 低于普通级别/较低
        /// </summary>
        BelowNormal,

        /// <summary>
        /// 正常
        /// </summary>
        Normal,

        /// <summary>
        /// 高于正常
        /// </summary>
        AboveNormal,

        /// <summary>
        /// 高
        /// </summary>
        High,

        /// <summary>
        /// 最高
        /// </summary>
        Highest
    }
}