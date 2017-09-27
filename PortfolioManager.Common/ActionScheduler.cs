using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortfolioManager.Common
{
    public class ActionScheduler
    {
        private List<ScheduledAction> _Actions;

        private bool _Active;
        public bool Active => _Active;

        private TimeSpan _BaseInterval;
        private CancellationTokenSource _CancelationTokenSource;

        public ActionScheduler()
        {
            _Actions = new List<ScheduledAction>();
            _Active = false;
            _BaseInterval = new TimeSpan(0, 0, 10);          
        }

        public void Start()
        {
            if (_Active)
                return;

            _Active = true;
            _CancelationTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => RunScheduledTasks(_CancelationTokenSource.Token));
        }

        public void Stop()
        {
            if (_Active)
            {
                _Active = false;
                _CancelationTokenSource.Cancel();
            }                  
        }

        private async Task RunScheduledTasks(CancellationToken cancelationToken)
        {
            while (true)
            {
                // Run any actions that are due
                foreach (var action in _Actions)
                {
                    if (cancelationToken.IsCancellationRequested)
                        break;

                    if (action.NextRun <= DateTime.Now)
                    {
                        action.Action();

                        action.NextRun = action.NextRun.Add(action.Interval);
                    }
                }

                // Remove expired items from the list
                _Actions.RemoveAll(x => x.RunUntil <= DateTime.Now);

                await Task.Delay(_BaseInterval, cancelationToken);

                if (cancelationToken.IsCancellationRequested)
                    break;
            }
        }

        public void RunAction(Action action)
        {
            AddRecurringAction(action, DateTime.Now, TimeSpan.MinValue, DateTime.Now);
        }

        public void AddRecurringAction(Action action, DateTime when, TimeSpan interval)
        {
            AddRecurringAction(action, when, interval, DateTime.MaxValue);
        }

        public void AddRecurringAction(Action action, DateTime when, TimeSpan interval, DateTime runUntil)
        {
            var task = new ScheduledAction()
            {
                Action = action,
                NextRun = when,
                Interval = interval,
                RunUntil = runUntil
            };

            _Actions.Add(task);
        }
    }

    class ScheduledAction
    {
        public Action Action;
        public DateTime NextRun;
        public TimeSpan Interval;
        public DateTime RunUntil;
    }
}
