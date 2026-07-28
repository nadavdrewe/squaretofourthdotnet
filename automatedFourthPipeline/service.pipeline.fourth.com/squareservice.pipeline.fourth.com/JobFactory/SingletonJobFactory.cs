using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;


namespace squareservice.pipeline.fourth.com.JobFactory
{
    public class SingletonJobFactory : IJobFactory
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public SingletonJobFactory(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var scope = _scopeFactory.CreateScope();
            var job = scope.ServiceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
            return new ScopedJob(job, scope);
        }

        public void ReturnJob(IJob job) { }

        private sealed class ScopedJob : IJob
        {
            private readonly IJob _innerJob;
            private readonly IServiceScope _scope;

            public ScopedJob(IJob innerJob, IServiceScope scope)
            {
                _innerJob = innerJob ?? throw new ArgumentNullException(nameof(innerJob));
                _scope = scope;
            }

            public async System.Threading.Tasks.Task Execute(IJobExecutionContext context)
            {
                try
                {
                    await _innerJob.Execute(context);
                }
                finally
                {
                    _scope.Dispose();
                }
            }
        }
    }
}
