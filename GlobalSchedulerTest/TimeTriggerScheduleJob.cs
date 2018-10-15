using System;
using System.Linq;

namespace GlobalSchedulerTest
{
	public class TimeTriggerScheduleJob : ScheduledJob
	{
		public TimeTrigger[] TriggerTimes { get; set; }

		public DayOfWeek[] Days { get; set; }

		protected override bool CanExecute(DateTime now)
		{
			if (this.TriggerTimes == null || this.TriggerTimes.Any() == false)
			{
				return false;
			}
			
			foreach (var trigger in this.TriggerTimes)
			{
				if (this.AtTime(trigger, now))
				{
					continue;
				}

				trigger.IsSet = false;
			}

			var timeTrigger = this.TriggerTimes.FirstOrDefault(t => this.AtTime(t, now));
			if (timeTrigger == null)
			{
				return false;
			}

			if (timeTrigger.IsSet)
			{
				return false;
			}

			timeTrigger.IsSet = true;
			
			return true;
		}

		private bool AtTime(TimeTrigger trigger, DateTime now)
		{
			// 초까지만 비교한다.
			var toSec = trigger.Time.Hours == now.Hour &&
			            trigger.Time.Minutes == now.Minute &&
			            trigger.Time.Seconds == now.Second;
			var isMatched = this.Days != null && this.Days.Length > 0 ? toSec && this.Days.Contains(now.DayOfWeek) : toSec;
			                
			return isMatched;
		}
	}
}
