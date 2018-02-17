using System;
using NUnit.Framework;

namespace Shuttle.Core.Cron.Tests
{
	[TestFixture]
	public class CronExpressionTest 
	{
		[Test]
		public void Should_be_able_to_create_an_all_inclusive_expression()
		{
			var date = new DateTime(2011, 1, 1, 0, 0, 0);
			var cron = new CronExpression("* * * * *", date);

			for (var i = 1; i < 121; i++)
			{
				Assert.AreEqual(date.AddMinutes(i), cron.NextOccurrence());
			}
		}

		[Test]
		public void Should_be_able_to_get_last_day_of_month_across_multiple_months()
		{
			var date = new DateTime(2011, 1, 1, 0, 0, 0);
			var cron = new CronExpression("0 0 L 1,2,5,6,9,12 *", date);

			Assert.AreEqual(new DateTime(2011, 1, 31, 0, 0, 0), cron.NextOccurrence());
			Assert.AreEqual(new DateTime(2011, 2, 28, 0, 0, 0), cron.NextOccurrence());
			Assert.AreEqual(new DateTime(2011, 5, 31, 0, 0, 0), cron.NextOccurrence());
			Assert.AreEqual(new DateTime(2011, 6, 30, 0, 0, 0), cron.NextOccurrence());
			Assert.AreEqual(new DateTime(2011, 9, 30, 0, 0, 0), cron.NextOccurrence());
			Assert.AreEqual(new DateTime(2011, 12, 31, 0, 0, 0), cron.NextOccurrence());

			date = new DateTime(2012, 1, 1, 0, 0, 0);
			cron = new CronExpression("0 0 L 1,2,5,6,9,12 *", date);

			Assert.AreEqual(new DateTime(2011, 12, 31, 0, 0, 0), cron.PreviousOccurrence());
			Assert.AreEqual(new DateTime(2011, 9, 30, 0, 0, 0), cron.PreviousOccurrence());
			Assert.AreEqual(new DateTime(2011, 6, 30, 0, 0, 0), cron.PreviousOccurrence());
			Assert.AreEqual(new DateTime(2011, 5, 31, 0, 0, 0), cron.PreviousOccurrence());
			Assert.AreEqual(new DateTime(2011, 2, 28, 0, 0, 0), cron.PreviousOccurrence());
			Assert.AreEqual(new DateTime(2011, 1, 31, 0, 0, 0), cron.PreviousOccurrence());
		}

		[Test]
		public void Should_be_able_to_get_week_days_across_month_boundary()
		{
			var date = new DateTime(2011, 1, 23, 0, 0, 0);
			var cron = new CronExpression("0 0 ? * 3,6", date);

			Assert.AreEqual(new DateTime(2011, 1, 25, 0, 0, 0), cron.NextOccurrence());
			Assert.AreEqual(new DateTime(2011, 1, 28, 0, 0, 0), cron.NextOccurrence());
			Assert.AreEqual(new DateTime(2011, 2, 1, 0, 0, 0), cron.NextOccurrence());
			Assert.AreEqual(new DateTime(2011, 2, 4, 0, 0, 0), cron.NextOccurrence());
			Assert.AreEqual(new DateTime(2011, 2, 8, 0, 0, 0), cron.NextOccurrence());

			date = new DateTime(2011, 2, 10, 0, 0, 0);
			cron = new CronExpression("0 0 ? * 3,6", date);

			Assert.AreEqual(new DateTime(2011, 2, 8, 0, 0, 0), cron.PreviousOccurrence());
			Assert.AreEqual(new DateTime(2011, 2, 4, 0, 0, 0), cron.PreviousOccurrence());
			Assert.AreEqual(new DateTime(2011, 2, 1, 0, 0, 0), cron.PreviousOccurrence());
			Assert.AreEqual(new DateTime(2011, 1, 28, 0, 0, 0), cron.PreviousOccurrence());
			Assert.AreEqual(new DateTime(2011, 1, 25, 0, 0, 0), cron.PreviousOccurrence());
		}

		[Test]
		public void Should_throw_exceptions_for_invalid_expressions()
		{
			Assert.Throws<CronException>(() => new CronExpression("* * *"));
			Assert.Throws<CronException>(() => new CronExpression("* * * * * *"));
			Assert.Throws<CronException>(() => new CronExpression("30-20 * * * * *"));
			Assert.Throws<CronException>(() => new CronExpression("* 1-24 * * * *"));
			Assert.Throws<CronException>(() => new CronExpression("* * X * *"));
			Assert.Throws<CronException>(() => new CronExpression("* * X * *"));
			Assert.Throws<CronException>(() => new CronExpression("* * ? * ?"));
			Assert.Throws<CronException>(() => new CronExpression("* * 1 * 1"));
		}
	}
}