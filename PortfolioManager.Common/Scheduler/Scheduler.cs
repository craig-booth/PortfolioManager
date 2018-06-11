using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PortfolioManager.Common.Scheduler
{
    public class Scheduler
    {
        private class ScheduledAction
        {
            public Action Action;
            public ISchedule Schedule;
            public DateTime NextRunTime;

            public ScheduledAction(Action action, ISchedule schedule, DateTime nextRunTime)
            {
                Action = action;
                Schedule = schedule;
                NextRunTime = nextRunTime;
            }
        }

        private List<ScheduledAction> _ScheduledActions = new List<ScheduledAction>();
        private CancellationTokenSource _CancellationTokenSource;
        public bool Running { get; private set; }

        public void Add(Action action, ISchedule schedule)
        {
            Add(action, schedule, DateTime.Now);
        }

        public void Add(Action action, ISchedule schedule, DateTime start)
        {
            _ScheduledActions.Add(new ScheduledAction(action, schedule, schedule.FirstRunTime(start)));

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

                var nextRunTime = DateTime.Now.AddDays(1);

                foreach (var scheduledAction in _ScheduledActions)
                {
                    if (scheduledAction.NextRunTime <= DateTime.Now)
                    {
                        ExecuteAction(scheduledAction.Action);
                        scheduledAction.NextRunTime = scheduledAction.Schedule.NextRunTime();
                    }

                    if (scheduledAction.NextRunTime <= nextRunTime)
                        nextRunTime = scheduledAction.NextRunTime;
                }

                _CancellationTokenSource = new CancellationTokenSource();
                Running = true;

                var delay = nextRunTime.Subtract(DateTime.Now);
                if (delay.Milliseconds > 0)
                {
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

        private void ExecuteAction(Action action)
        {
            Task.Run(action);
        }
    }
}
