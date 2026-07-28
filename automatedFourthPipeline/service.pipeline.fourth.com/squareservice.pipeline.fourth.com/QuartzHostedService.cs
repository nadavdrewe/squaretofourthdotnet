using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Spi;
using ILArmyLogistics.Worker;

namespace squareservice.pipeline.fourth.com
{

    public class QuartzHostedService : BackgroundService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly IEnumerable<Schedule> _jobSchedules;
        private readonly IConfiguration _configuration;

        public QuartzHostedService(
            ISchedulerFactory schedulerFactory,
            IJobFactory jobFactory,
            IEnumerable<Schedule> jobSchedules,
            IConfiguration configuration)
        {
            _schedulerFactory = schedulerFactory;
            _jobSchedules = jobSchedules;
            _jobFactory = jobFactory;
            _configuration = configuration;
        }
        public IScheduler Scheduler { get; set; }


        ///// <summary>
        ///// This is where we work
        ///// </summary>
        ///// <param name="cancellationToken"></param>
        ///// <returns></returns>
        //public async Task StartAsync(CancellationToken cancellationToken)
        //{
           
        //}

        //public async Task StopAsync(CancellationToken cancellationToken)
        //{
        //    await Scheduler?.Shutdown(cancellationToken);
        //}

        private static IJobDetail CreateJob(Schedule schedule)
        {
            var jobType = schedule.JobType;
            return JobBuilder
                .Create(jobType)
                .WithIdentity(jobType.FullName)
                .WithDescription(jobType.Name)
                .Build();
        }

        private static ITrigger CreateTrigger(Schedule schedule)
        {
            return TriggerBuilder
                .Create()
                .WithIdentity($"{schedule.JobType.FullName}.trigger")
                .WithCronSchedule(schedule.CronExpression)
                .WithDescription(schedule.CronExpression)
                .Build();
        }


        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Scheduler = await _schedulerFactory.GetScheduler(stoppingToken);
            Scheduler.JobFactory = _jobFactory;

            foreach (var jobSchedule in _jobSchedules)
            {
                var job = CreateJob(jobSchedule);
                var trigger = CreateTrigger(jobSchedule);

                await Scheduler.ScheduleJob(job, trigger, stoppingToken);
            }

            await Scheduler.Start(stoppingToken);

            if (_configuration.GetValue<bool>("SquareToFourthSales:RunOnStartup", false))
            {
                foreach (var jobSchedule in _jobSchedules)
                {
                    await Scheduler.TriggerJob(new JobKey(jobSchedule.JobType.FullName), stoppingToken);
                }
            }
        }
    }
}
