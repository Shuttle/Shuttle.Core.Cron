using System;

namespace Shuttle.Core.Cron
{
	public class CronHour : CronField
	{
		public CronHour(string expression, ISpecificationFactory specificationFactory = null) : base(expression, specificationFactory)
		{
			DefaultParsing(FieldName.Hour, 0, 23);
		}

		public override DateTime GetNext(DateTime date)
		{
			while (!IsSatisfiedBy(new Candidate(FieldName.Hour, Expression, date)))
			{
				date = date.AddHours(1);
			}

			return date;
		}

		public override DateTime GetPrevious(DateTime date)
		{
			while (!IsSatisfiedBy(new Candidate(FieldName.Hour, Expression, date)))
			{
				date = date.AddHours(-1);
			}

			return date;
		}
	}
}