using System;
using System.Linq;

namespace GlobalSchedulerTest
{
	public class IntervalScheduleJob : ScheduledJob
	{
		public int Interval { get; set; }

		public TimeUnit Unit { get; set; }

		public bool UseTimeRange { get; set; }

		public TimeSpan StartTime { get; set; }

		public TimeSpan EndTime { get; set; }

		public DayOfWeek[] Days { get; set; }

		protected override bool CanExecute(DateTime now)
		{
			var last = this.LastExecutedTime;
			if (last == default(DateTime))
			{
				return true;
			}

			var interval = this.Interval * this.Unit;
			var gap = now - last;
			var inThisTime = gap > interval;

			if (inThisTime == false)
			{
				return false;
			}

			// 이하 interval 이내 들어온 상황.

			if (this.UseTimeRange == false)
			{
				return true;
			}

			//var between = now > this.StartTime && now < this.EndTime;
			var between = this.AtTime(this.StartTime, this.EndTime, now);
			if (between == false)
			{
				return false;
			}
			
			if (this.Days == null || this.Days.Any() == false)
			{
				// TimeRange만 설정하고 Day를 설정하지 않은 경우.
				// 매일 같은 TimeRange 에 해당 interval로 schedule 되는 것을 의미함.
				return true;
			}

			var inDay = this.Days.Any(d => d == now.DayOfWeek);
			if (inDay == false)
			{
				// 지정된 Day 가 아님.
				return false;
			}

			return true;
		}

		private bool AtTime(TimeSpan startTime, TimeSpan endTime, DateTime now)
		{
			var between = endTime - startTime;
			if (default(TimeSpan) == between)
			{
				// start / end time에 값이 설정되어 있지 않으면 그냥 스케줄링한다.
				return true;
			}

			var interval = this.Interval * this.Unit;
			if (interval > between)
			{
				// interval 은 지정된 시간 간격보다 크면 안 된다.
				return false;
			}

			// 시간, 분, 초 비교.
			return now.TimeOfDay > startTime && now.TimeOfDay < endTime;
		}
	}
}
