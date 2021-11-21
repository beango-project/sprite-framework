using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Sprite.Scheduling.Abstractions
{
    public class SchedulingJobWorker
    {
        private readonly ISchedulingSerializer _serializer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ISchedulingJobStore _store;

        public SchedulingJobWorker(ISchedulingJobStore store, ISchedulingSerializer serializer)
        {
            _store = store;
            _serializer = serializer;
        }

        public string TaskName { get; }

        public async Task DoWorkAsync()
        {
            var jobs = _store.GetScheduledJobs();


            jobs.ForEach(x =>
            {
                var jobExecuteMethod = x.JobType.GetMethod(nameof(ISchedulingJob<object>.Execute)) ??
                                       x.JobType.GetMethod(nameof(IAsyncSchedulingJob<object>.ExecuteAsync));
                var jobObject = _serializer.Deserialize(x.JobContent, x.JobType);
                var jobContent = _serializer.Deserialize(x.JobContent, x.JobContentType);
                jobExecuteMethod.Invoke(jobObject, new[] {jobContent});
                // jobExecuteMethod.Invoke(x,new { (object)x.JobArgs})
            });
            await Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}