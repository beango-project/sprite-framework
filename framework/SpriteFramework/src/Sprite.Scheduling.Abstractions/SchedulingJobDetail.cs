using System;

namespace Sprite.Scheduling.Abstractions
{
    public class SchedulingJobDetail
    {
        public Guid Id { get; set; }

        public string JobName { get; set; }

        public Type JobType { get; set; }

        public string JobContent { get; set; }

        public Type JobContentType { get; set; }

        /// <summary>
        /// Creation time of this job.
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        public SchedulingJobPriority Priority { get; set; }
    }
}