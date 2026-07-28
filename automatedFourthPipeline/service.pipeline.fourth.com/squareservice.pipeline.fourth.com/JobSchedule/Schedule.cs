using System;
using System.Collections.Generic;
using System.Text;

namespace ILArmyLogistics.Worker
{
    public class Schedule
    {
        /// <summary>
        /// Used to schedule a IJob
        /// </summary>
        /// <param name="jobType"></param>
        /// <param name="cronExpression"></param>
        public Schedule(Type jobType, string cronExpression)
        {
            JobType = jobType;
            CronExpression = cronExpression;
        }

        public Type JobType { get; }
        public string CronExpression { get; }
    }
}
