using System;
using NUnit.Framework;

namespace Shuttle.Core.Cron.Tests
{
	[TestFixture]
	public class CronHourTest
	{
		[Test]
		public void Should_be_able_to_satisfy_asterisk_value()
		{
			var field = new CronHour("*");

			var date = new DateTime(2011, 01, 01, 0, 0, 0);

			for (var i = 0; i < 24; i++)
			{
				Assert.AreEqual(date, field.SnapForward(date));

				date = date.AddHours(1);
			}
		}

		[Test]
		public void Should_be_able_to_satisfy_individual_values()
		{
			var field = new CronHour("5,10,15,20");

			var control = new DateTime(2011, 01, 01, 0, 0, 0);
			var date = field.SnapForward(control);

			Assert.AreEqual(control.AddHours(5), date);
			date = field.SnapForward(date.AddHours(1));

			Assert.AreEqual(control.AddHours(10), date);
			date = field.SnapForward(date.AddHours(1));

			Assert.AreEqual(control.AddHours(15), date);
			date = field.SnapForward(date.AddHours(1));

			Assert.AreEqual(control.AddHours(20), date);
			date = field.SnapForward(date.AddHours(1));

			Assert.AreEqual(control.AddHours(29), date);
		}

		[Test]
		public void Should_be_able_to_satisfy_a_range_of_values()
		{
			var field = new CronHour("5-10");

			var control = new DateTime(2011, 01, 01, 0, 0, 0);
			var date = field.SnapForward(control);

			Assert.AreEqual(control.AddHours(5), date);
			date = field.SnapForward(date.AddHours(1));

			Assert.AreEqual(control.AddHours(6), date);
			date = field.SnapForward(date.AddHours(1));

			Assert.AreEqual(control.AddHours(7), date);
			date = field.SnapForward(date.AddHours(1));

			Assert.AreEqual(control.AddHours(8), date);
			date = field.SnapForward(date.AddHours(1));

			Assert.AreEqual(control.AddHours(9), date);
			date = field.SnapForward(date.AddHours(1));

			Assert.AreEqual(control.AddHours(10), date);
		}

		[Test]
		public void Should_be_able_to_satisfy_a_stepped_values()
		{
			var field = new CronHour("5-10/5");

			var control = new DateTime(2011, 01, 01, 0, 0, 0);
			var date = field.SnapForward(control);

			Assert.AreEqual(control.AddHours(5), date);
			date = field.SnapForward(date.AddHours(1));

			Assert.AreEqual(control.AddHours(10), date);
			date = field.SnapForward(date.AddHours(1));

			Assert.AreEqual(control.AddHours(29), date);
		}

		[Test]
		public void Should_throw_exceptions_on_invalid_expressions()
		{
			Assert.Throws<NullReferenceException>(() => new CronHour(""));
			Assert.Throws<CronException>(() => new CronHour("invalid"));
			Assert.Throws<CronException>(() => new CronHour("10-60"));
			Assert.Throws<CronException>(() => new CronHour("60-60"));
			Assert.Throws<CronException>(() => new CronHour("10-5"));

			new CronHour("*/15");
		}
	}
}