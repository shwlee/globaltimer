using System;

namespace GlobalSchedulerTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var scheduler = new GlobalScheduler();

			scheduler.Start();

			Console.WriteLine("Start!");

			Console.ReadLine();
			
			var interval1 = new IntervalScheduleJob();
			interval1.Interval = 1000;
			interval1.Unit = TimeUnits.MilliSecond;
			interval1.Execute = () =>
			{
				var now = DateTime.Now;
				Console.WriteLine($"~~ Interval1   Invoke! : {interval1.Id}, {now}");
			};

			var id1 = scheduler.Register(interval1);

			Console.WriteLine("Interval1 Registered!");

			Console.ReadLine();

			var interval2 = new IntervalScheduleJob();
			interval2.Interval = 3;
			interval2.Unit = TimeUnits.Second;
			interval2.Execute = () =>
			{
				var now = DateTime.Now;
				Console.WriteLine($"Interval2 ~~   Invoke! : {interval2.Id}, {now}");
			};

			var id2 = scheduler.Register(interval2);

			Console.WriteLine("Interval2 Registered!!");

			Console.ReadLine();

			var interval3 = new IntervalScheduleJob();
			interval3.Interval = 1;
			interval3.Unit = TimeUnits.Second;
			interval3.UseTimeRange = true;
			interval3.StartTime = DateTime.Parse("10/15/2018 14:34:10");
			interval3.EndTime = DateTime.Parse("10/15/2018 14:34:20");
			interval3.Execute = () =>
			{
				var now = DateTime.Now;
				Console.WriteLine($" ~~Interval3~~ Invoke! : {interval3.Id}, {now}");
			};

			var id3 = scheduler.Register(interval3);
			Console.WriteLine("Interval3 Registered!!!");

			Console.ReadLine();

			scheduler.Unregister(id1);
			Console.WriteLine("Interval1 Unregistered!");

			Console.ReadLine();

			scheduler.Unregister(id2);
			Console.WriteLine("Interval2 Unregistered!!");

			Console.ReadLine();

			scheduler.Unregister(id3);
			Console.WriteLine("Interval3 Unregistered!!!");

			Console.ReadLine();

			Console.WriteLine("Stop!");

			scheduler.Dispose();

			Console.WriteLine("Scheduler disposed!");

			Console.ReadLine();
		}
	}
}
