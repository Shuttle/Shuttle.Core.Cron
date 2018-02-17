using System;

namespace Shuttle.Core.Cron
{
	public class CronHour : CronField
	{
		public CronHour(string value) : base(value)
		{
			DefaultParsing(0, 23);
		}

		public override DateTime GetNext(DateTime date)
		{
			while (!IsSatisfiedBy(date.Hour))
			{
				date = date.AddHours(1);
			}

			return date;
		}

		public override DateTime GetPrevious(DateTime date)
		{
			while (!IsSatisfiedBy(date.Hour))
			{
				date = date.AddHours(-1);
			}

			return date;
		}
	}
}