using System;

namespace Shuttle.Core.Cron
{
	public class CronMinute : CronField
	{
		public CronMinute(string value) : base(value)
		{
			DefaultParsing(0, 59);
		}

		public override DateTime GetNext(DateTime date)
		{
			while (!IsSatisfiedBy(date.Minute))
			{
				date = date.AddMinutes(1);
			}

			return date;
		}

		public override DateTime GetPrevious(DateTime date)
		{
			while (!IsSatisfiedBy(date.Minute))
			{
				date = date.AddMinutes(-1);
			}

			return date;
		}
	}
}