using System;

namespace GlobalSchedulerTest
{
	public abstract class ScheduledJob
	{
		public uint Id { get; internal set; }

		public DateTime LastExecutedTime { get; set; }

		public Action Execute { get; set; }

		public abstract bool CanExecuteNow();
		
		public void ExecuteJob()
		{
			this.Execute?.Invoke();
			this.LastExecutedTime = DateTime.Now;
		}
	}
}
