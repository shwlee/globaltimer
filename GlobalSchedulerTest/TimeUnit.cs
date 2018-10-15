using System;

namespace GlobalSchedulerTest
{
	public static class TimeUnits
	{
		public static TimeUnit MilliSecond { get; } = new TimeUnit(TimeSpan.FromMilliseconds(1));
		public static TimeUnit Second { get; } = new TimeUnit(TimeSpan.FromSeconds(1));
		public static TimeUnit Minuite { get; } = new TimeUnit(TimeSpan.FromMinutes(1));
		public static TimeUnit Hour { get; } = new TimeUnit(TimeSpan.FromHours(1));
		public static TimeUnit Day { get; } = new TimeUnit(TimeSpan.FromDays(1));
	}

	public class TimeUnit
	{
		public TimeUnit(TimeSpan value)
		{
			Value = value;
		}

		public TimeSpan Value { get; private set; }

		public static TimeSpan operator *(int amount, TimeUnit unit)
		{
			return new TimeSpan(amount * unit.Value.Ticks);
		}

		public static TimeSpan operator *(double amount, TimeUnit unit)
		{
			return TimeSpan.FromMilliseconds(amount * unit.Value.TotalMilliseconds);
		}
	}
}
