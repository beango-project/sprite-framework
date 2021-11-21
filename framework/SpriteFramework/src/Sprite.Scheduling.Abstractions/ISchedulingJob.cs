using System;

namespace Sprite.Scheduling.Abstractions
{
    public interface ISchedulingJob<TContext>
    {
        void Execute(TContext context);
    }

    public class EmailSendContext
    {
        public string TaskName => "SendEmail";

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }

    public class EmailSendJob : SchedulingJob<EmailSendContext>
    {
        public override void Execute(EmailSendContext context)
        {
            Console.WriteLine(nameof(EmailSendJob));
        }
    }
}