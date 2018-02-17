using System;

namespace Shuttle.Core.Cron
{
	public class CronMinute : CronField
	{
		public CronMinute(string value) : base(value)
		{
			DefaultParsing(0, 59);
		}

		public override DateTime SnapForward(DateTime date)
		{
			while (!IsSatisfiedBy(date.Minute))
			{
				date = date.AddMinutes(1);
			}

			return date;
		}

		public override DateTime SnapBackward(DateTime date)
		{
			while (!IsSatisfiedBy(date.Minute))
			{
				date = date.AddMinutes(-1);
			}

			return date;
		}
	}
}