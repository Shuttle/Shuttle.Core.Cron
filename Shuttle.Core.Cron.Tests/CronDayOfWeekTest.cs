using System;
using NUnit.Framework;

namespace Shuttle.Core.Cron.Tests
{
	[TestFixture]
	public class CronDayOfWeekTest
	{
		[Test]
		public void Should_be_able_to_satisfy_asterisk_value()
		{
			var field = new CronDayOfWeek("*");

			var date = new DateTime(2011, 01, 01, 0, 0, 0);

			for (var i = 0; i < 8; i++)
			{
				Assert.AreEqual(date, field.SnapForward(date));

				date = date.AddDays(1);
			}
		}

		[Test]
		public void Should_be_able_to_satisfy_last_day_of_week()
		{
			var field = new CronDayOfWeek("L");

			var control = new DateTime(2011, 01, 01, 0, 0, 0);
			var date = field.SnapForward(control);

			Assert.AreEqual(control, date);
			date = field.SnapForward(date.AddDays(1));

			Assert.AreEqual(control.AddDays(7), date);
			date = field.SnapForward(date.AddDays(1));

			Assert.AreEqual(control.AddDays(14), date);
			date = field.SnapForward(date.AddDays(1));

			Assert.AreEqual(control.AddDays(21), date);
			date = field.SnapForward(date.AddDays(1));

			Assert.AreEqual(control.AddDays(28), date);
		}

		[Test]
		public void Should_be_able_to_satisfy_occurrence()
		{
			var field = new CronDayOfWeek("4#2");

			var date = new DateTime(2011, 01, 01, 0, 0, 0);

			Assert.AreEqual(date.AddDays(11), field.SnapForward(date));
		}

		[Test]
		public void Should_be_able_to_satisfy_individual_values()
		{
			var field = new CronDayOfWeek("1,3,6");

			var control = new DateTime(2011, 01, 01, 0, 0, 0);
			var date = field.SnapForward(control);

			Assert.AreEqual(control.AddDays(1), date);
			date = field.SnapForward(date.AddDays(1));

			Assert.AreEqual(control.AddDays(3), date);
			date = field.SnapForward(date.AddDays(1));

			Assert.AreEqual(control.AddDays(6), date);
			date = field.SnapForward(date.AddDays(1));

			Assert.AreEqual(control.AddDays(8), date);
			date = field.SnapForward(date.AddDays(1));

			Assert.AreEqual(control.AddDays(10), date);
			date = field.SnapForward(date.AddDays(1));
			
			Assert.AreEqual(control.AddDays(13), date);
		}

		[Test]
		public void Should_be_able_to_satisfy_a_range_of_values()
		{
			var field = new CronDayOfWeek("2-4");

			var control = new DateTime(2011, 01, 01, 0, 0, 0);
			var date = field.SnapForward(control);

			Assert.AreEqual(control.AddDays(2), date);
			date = field.SnapForward(date.AddDays(1));

			Assert.AreEqual(control.AddDays(3), date);
			date = field.SnapForward(date.AddDays(1));

			Assert.AreEqual(control.AddDays(4), date);
			date = field.SnapForward(date.AddDays(1));

			Assert.AreEqual(control.AddDays(9), date);
			date = field.SnapForward(date.AddDays(1));

			Assert.AreEqual(control.AddDays(10), date);
			date = field.SnapForward(date.AddDays(1));

			Assert.AreEqual(control.AddDays(11), date);
		}

		[Test]
		public void Should_be_able_to_satisfy_a_stepped_values()
		{
			var field = new CronDayOfWeek("2/5");

			var control = new DateTime(2011, 01, 01, 0, 0, 0);
			var date = field.SnapForward(control);

			Assert.AreEqual(control, date);
			date = field.SnapForward(date.AddDays(1));

			Assert.AreEqual(control.AddDays(2), date);
			date = field.SnapForward(date.AddDays(1));

			Assert.AreEqual(control.AddDays(7), date);
			date = field.SnapForward(date.AddDays(1));

			Assert.AreEqual(control.AddDays(9), date);
			date = field.SnapForward(date.AddDays(1));

			Assert.AreEqual(control.AddDays(14), date);
		}

		[Test]
		public void Should_throw_exceptions_on_invalid_expressions()
		{
			Assert.Throws<NullReferenceException>(() => new CronDayOfWeek(""));
			Assert.Throws<CronException>(() => new CronDayOfWeek("invalid"));
			Assert.Throws<CronException>(() => new CronDayOfWeek("10-60"));
			Assert.Throws<CronException>(() => new CronDayOfWeek("60-60"));
			Assert.Throws<CronException>(() => new CronDayOfWeek("10-5"));

			new CronDayOfWeek("*/15");
		}
	}
}