using System;

namespace GlobalSchedulerTest
{
	public abstract class ScheduledJob
	{
		public uint Id { get; internal set; }

		public DateTime LastExecutedTime { get; internal set; }

		public Action Execute { get; set; }

		protected abstract bool CanExecute(DateTime now);

		public void DoJob(DateTime now)
		{
			if (this.CanExecuteNow(now) == false)
			{
				return;
			}

			this.LastExecutedTime = DateTime.Now;

			this.ExecuteJob();
		}

		private bool CanExecuteNow(DateTime now)
		{
			var canExecute = this.CanExecute(now);

			return canExecute;
		}

		private void ExecuteJob()
		{
			this.Execute?.Invoke();
		}
	}
}
