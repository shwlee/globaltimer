using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GlobalSchedulerTest
{
	public class GlobalScheduler : IDisposable
	{
		#region	Fields

		private static int _idSequence;

		private readonly List<ScheduledJob> _jobs = new List<ScheduledJob>();

		private Task _schedulingTask;

		private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		private object _syncBlock = new object();

		#endregion

		#region Properties

		public bool IsScheduleRunning
		{
			get
			{
				if (this._schedulingTask == null)
				{
					return false;
				}

				return this._schedulingTask.Status == TaskStatus.Running;
			}
		}

		#endregion

		#region Medthods

		public void Start()
		{
			if (this._schedulingTask != null)
			{
				this._cancellationTokenSource.Cancel();
				this._schedulingTask.Dispose();
				this._schedulingTask = null;

				this._cancellationTokenSource.Dispose();
				this._cancellationTokenSource = null;
			}

			this._cancellationTokenSource = new CancellationTokenSource();
			this._schedulingTask = new Task(this.Scheduling, this._cancellationTokenSource);
			this._schedulingTask.Start();
		}

		public void Stop()
		{
			this._cancellationTokenSource?.Cancel();
		}

		public uint Register(ScheduledJob job)
		{
			lock (this._syncBlock)
			{
				// TODO : need lock
				job.Id = this.IncrementCount();
				job.LastExecutedTime = DateTime.Now;

				this._jobs.Add(job);

				return job.Id;
			}
		}

		private uint IncrementCount()
		{
			var newValue = Interlocked.Increment(ref _idSequence);
			return unchecked((uint)newValue);
		}

		public void Unregister(uint jobId)
		{
			this.Unregisterinternal(jobId);
		}

		public void Unregister(ScheduledJob job)
		{
			this.Unregister(job.Id);
		}

		private async void Scheduling(object cancelSource)
		{
			if (!(cancelSource is CancellationTokenSource cancelation))
			{
				return;
			}

			try
			{
				while (true)
				{
					this.CheckTaskCanceled(cancelation);

					await Task.Delay(100, cancelation.Token);

					this.CheckTaskCanceled(cancelation);

					List<ScheduledJob> jobs;

					lock (this._syncBlock)
					{
						jobs = this._jobs.ToList();
					}

					if (jobs == null)
					{
						throw new NullReferenceException("There is nothing that registered scheduled job.");
					}

					// check and execute.
					var now = DateTime.Now;
					foreach (var job in jobs)
					{
						// 개별 job 이 수행되는 동안 Scheduling task 가 영향을 받지 않도록 한다.
						Task.Run(() =>
						{

							if (job.CanExecuteNow() == false)
							{
								return;
							}

							job?.ExecuteJob();

						});
					}
				}
			}
			catch (Exception ex)
			{
				// TODO : need logging.
			}
		}

		private void CheckTaskCanceled(CancellationTokenSource cancelSource)
		{
			if (cancelSource.Token.IsCancellationRequested == false)
			{
				return;
			}

			// Clean up here, then...
			cancelSource.Token.ThrowIfCancellationRequested();
		}

		private void Unregisterinternal(uint jobId)
		{
			lock (this._syncBlock)
			{
				this._jobs.RemoveAll(j => j.Id == jobId);
			}
		}

		public void Dispose()
		{
			this.Stop();

			this._jobs.Clear();
		}

		#endregion
	}
}
