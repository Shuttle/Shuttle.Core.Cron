using System;
using NUnit.Framework;

namespace Shuttle.Core.Cron.Tests
{
	[TestFixture]
	public class CronMonthTest
	{
		[Test]
		public void Should_be_able_to_satisfy_asterisk_value()
		{
			var field = new CronMonth("*");

			var date = new DateTime(2011, 01, 01, 0, 0, 0);

			for (var i = 1; i < 13; i++)
			{
				Assert.AreEqual(date, field.SnapForward(date));

				date = date.AddHours(1);
			}
		}

		[Test]
		public void Should_be_able_to_satisfy_individual_values()
		{
			var field = new CronMonth("2,APR,May,8,oct");

			var control = new DateTime(2011, 01, 01, 0, 0, 0);
			var date = field.SnapForward(control);

			Assert.AreEqual(control.AddMonths(1), date);
			date = field.SnapForward(date.AddMonths(1));

			Assert.AreEqual(control.AddMonths(3), date);
			date = field.SnapForward(date.AddMonths(1));

			Assert.AreEqual(control.AddMonths(4), date);
			date = field.SnapForward(date.AddMonths(1));

			Assert.AreEqual(control.AddMonths(7), date);
			date = field.SnapForward(date.AddMonths(1));

			Assert.AreEqual(control.AddMonths(9), date);
		}

		[Test]
		public void Should_be_able_to_satisfy_a_range_of_values()
		{
			var field = new CronMonth("5-10");

			var control = new DateTime(2011, 01, 01, 0, 0, 0);
			var date = field.SnapForward(control);

			Assert.AreEqual(control.AddMonths(4), date);
			date = field.SnapForward(date.AddMonths(1));

			Assert.AreEqual(control.AddMonths(5), date);
			date = field.SnapForward(date.AddMonths(1));

			Assert.AreEqual(control.AddMonths(6), date);
			date = field.SnapForward(date.AddMonths(1));

			Assert.AreEqual(control.AddMonths(7), date);
			date = field.SnapForward(date.AddMonths(1));

			Assert.AreEqual(control.AddMonths(8), date);
			date = field.SnapForward(date.AddMonths(1));

			Assert.AreEqual(control.AddMonths(9), date);
		}

		[Test]
		public void Should_be_able_to_satisfy_a_stepped_values()
		{
			var field = new CronMonth("5-10/5");

			var control = new DateTime(2011, 01, 01, 0, 0, 0);
			var date = field.SnapForward(control);

			Assert.AreEqual(control.AddMonths(4), date);
			date = field.SnapForward(date.AddMonths(1));

			Assert.AreEqual(control.AddMonths(9), date);
			date = field.SnapForward(date.AddMonths(1));

			Assert.AreEqual(control.AddMonths(16), date);
		}

		[Test]
		public void Should_throw_exceptions_on_invalid_expressions()
		{
			Assert.Throws<NullReferenceException>(() => new CronMonth(""));
			Assert.Throws<CronException>(() => new CronMonth("invalid"));
			Assert.Throws<CronException>(() => new CronMonth("10-60"));
			Assert.Throws<CronException>(() => new CronMonth("60-60"));
			Assert.Throws<CronException>(() => new CronMonth("10-5"));

			new CronMonth("*/15");
		}
	}
}