using System;

namespace Shuttle.Core.Cron
{
	public class CronHour : CronField
	{
		public CronHour(string value) : base(value)
		{
			DefaultParsing(0, 23);
		}

		public override DateTime SnapForward(DateTime date)
		{
			while (!IsSatisfiedBy(date.Hour))
			{
				date = date.AddHours(1);
			}

			return date;
		}

		public override DateTime SnapBackward(DateTime date)
		{
			while (!IsSatisfiedBy(date.Hour))
			{
				date = date.AddHours(-1);
			}

			return date;
		}
	}
}