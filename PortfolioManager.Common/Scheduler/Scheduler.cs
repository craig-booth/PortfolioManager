using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace PortfolioManager.Common.Scheduler
{
    public class Scheduler
    {
        private class ScheduledTask
        {
            public string Name;
            public Action Action;
            public ISchedule Schedule;
            public DateTime NextRunTime;

            public ScheduledTask(string name, Action action, ISchedule schedule, DateTime nextRunTime)
            {
                Name = name;
                Action = action;
                Schedule = schedule;
                NextRunTime = nextRunTime;
            }
        }

        private List<ScheduledTask> _ScheduledTasks;
        private CancellationTokenSource _CancellationTokenSource;
        private ILogger _Logger;

        public bool Running { get; private set; }

        public Scheduler()
        {
            _ScheduledTasks  = new List<ScheduledTask>();
            Running = false;
        }

        public Scheduler(ILogger<Scheduler> logger)
            : this()
        {
            _Logger = logger;
        }

        public void Add(string name, Action action, ISchedule schedule)
        {
            Add(name, action, schedule, DateTime.Now);
        }

        public void Add(string name, Action action, ISchedule schedule, DateTime start)
        {
            var task = new ScheduledTask(name, action, schedule, schedule.FirstRunTime(start));
            _ScheduledTasks.Add(task);

            _Logger.LogInformation("Task {0} added - {1}. First run time {2}", task.Name, task.Schedule.ToString(), task.NextRunTime);

            // Cancel delay so that new task can be checked
            if (Running)
                _CancellationTokenSource.Cancel();
        }

        public Task Run()
        {
            return Run(CancellationToken.None);
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                _Logger?.LogTrace("Checking for scheduled tasks");

                var nextRunTime = DateTime.Now.AddDays(1);

                foreach (var scheduledTask in _ScheduledTasks)
                {
                    if (scheduledTask.NextRunTime <= DateTime.Now)
                    {
                        ExecuteAction(scheduledTask);
                        scheduledTask.NextRunTime = scheduledTask.Schedule.NextRunTime();
                    }

                    if (scheduledTask.NextRunTime <= nextRunTime)
                        nextRunTime = scheduledTask.NextRunTime;
                }

                _CancellationTokenSource = new CancellationTokenSource();
                Running = true;

                var delay = nextRunTime.AddSeconds(-5).Subtract(DateTime.Now);
                if (delay.Milliseconds > 0)
                {
                    _Logger?.LogInformation("Sleeping for {0}", delay.ToString());

                    try
                    {
                        await Task.Delay(delay, CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _CancellationTokenSource.Token).Token);
                    }
                    catch (TaskCanceledException) 
                    {
                        //Ignore
                    }
                }
                    
            }
        }

        private void ExecuteAction(ScheduledTask task)
        {
            _Logger?.LogInformation("Executing task {0}", task.Name);
            Task.Run(task.Action);
        }
    }
}
