using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		private Func<DateTime> _getNow;

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

			this._getNow = this.GetCurrentNow;

			this._cancellationTokenSource = new CancellationTokenSource();
			this._schedulingTask = new Task(this.DoScheduling, this._cancellationTokenSource);
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

		public void Unregister(uint jobId)
		{
			this.Unregisterinternal(jobId);
		}

		public void Unregister(ScheduledJob job)
		{
			this.Unregister(job.Id);
		}

		public void SetNowTime(Func<DateTime> getNow)
		{
			this._getNow = getNow;
		}

		private DateTime GetCurrentNow()
		{
			return DateTime.Now;
		}

		private uint IncrementCount()
		{
			var newValue = Interlocked.Increment(ref _idSequence);
			return unchecked((uint)newValue);
		}

		private async void DoScheduling(object cancelSource)
		{
			if (!(cancelSource is CancellationTokenSource cancelation))
			{
				return;
			}

			while (true)
			{
				try
				{
					this.CheckTaskCanceled(cancelation);

					await Task.Delay(85, cancelation.Token); // Windows 기본 타이머 15.625ms 간격을 제외한 100ms 단위.

					Debug.WriteLine($"[shwlee] Thread:{Thread.CurrentThread.ManagedThreadId}");

					this.CheckTaskCanceled(cancelation);

					List<ScheduledJob> jobs;

					lock (this._syncBlock)
					{
						jobs = this._jobs.ToList();
					}
					
					// check and execute.
					var now = this._getNow?.Invoke() ?? this.GetCurrentNow();
					foreach (var job in jobs)
					{
						// 개별 job 이 수행되는 동안 Scheduling task 가 영향을 받지 않도록 한다.
						Task.Run(() => job.DoJob(now));
					}
				}
				catch (Exception ex)
				{
					// TODO : need logging.
					Debug.WriteLine(ex);
				}
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
