using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Sprite.Scheduling.Abstractions;

namespace Sprite.Scheduling
{
    public class MemorySchedulingJobStore : ISchedulingJobStore
    {
        private readonly ConcurrentDictionary<Guid, SchedulingJobDetail> _store;

        public MemorySchedulingJobStore()
        {
            _store = new ConcurrentDictionary<Guid, SchedulingJobDetail>();
        }

        public void Add(SchedulingJobDetail jobDetail)
        {
            _store[jobDetail.Id] = jobDetail;
        }

        public List<SchedulingJobDetail> GetScheduledJobs()
        {
            return _store.Values.OrderByDescending(x => x.Priority).ToList();
        }
    }
}