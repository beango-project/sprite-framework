namespace Sprite.Scheduling.Abstractions
{
    public abstract class SchedulingJob<TContext> : ISchedulingJob<TContext>
    {
        // public ILogger<SchedulingJob<TContext>> Logger { get; set; }


        public abstract void Execute(TContext context);
    }
}