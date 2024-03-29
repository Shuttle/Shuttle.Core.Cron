using System;
using NUnit.Framework;
using Shuttle.Core.Specification;

namespace Shuttle.Core.Cron.Tests
{
	[TestFixture]
	public class CronDayOfMonthTest
	{
		[Test]
		public void Should_be_able_to_satisfy_asterisk_value()
		{
			var field = new CronDayOfMonth("*");

			var date = new DateTime(2011, 01, 01, 0, 0, 0);

			for (var i = 0; i < 32; i++)
			{
				Assert.AreEqual(date, field.GetNext(date));

				date = date.AddDays(1);
			}
		}

		[Test]
		public void Should_be_able_to_satisfy_last_day_of_month()
		{
			var field = new CronDayOfMonth("L");

			var date = new DateTime(2011, 01, 01, 0, 0, 0);

			Assert.AreEqual(date.AddDays(30), field.GetNext(date));
		}

		[Test]
		public void Should_be_able_to_satisfy_absolute_weekday()
		{
			var field = new CronDayOfMonth("12W");

			var date = new DateTime(2011, 01, 01, 0, 0, 0);

			Assert.AreEqual(date.AddDays(11), field.GetNext(date));
		}

		[Test]
		public void Should_be_able_to_satisfy_midmonth_saturday_weekday()
		{
			var field = new CronDayOfMonth("15W");

			var date = new DateTime(2011, 01, 01, 0, 0, 0);

			Assert.AreEqual(date.AddDays(13), field.GetNext(date));
		}

		[Test]
		public void Should_be_able_to_satisfy_midmonth_sunday_weekday()
		{
			var field = new CronDayOfMonth("16W");

			var date = new DateTime(2011, 01, 01, 0, 0, 0);

			Assert.AreEqual(date.AddDays(16), field.GetNext(date));
		}

		[Test]
		public void Should_be_able_to_satisfy_firstday_saturday_weekday()
		{
			var field = new CronDayOfMonth("1W");

			var date = new DateTime(2011, 01, 01, 0, 0, 0);

			Assert.AreEqual(date.AddDays(2), field.GetNext(date));
		}

		[Test]
		public void Should_be_able_to_satisfy_lastday_sunday_weekday()
		{
			var field = new CronDayOfMonth("31W");

			var date = new DateTime(2011, 07, 01, 0, 0, 0);

			Assert.AreEqual(date.AddDays(28), field.GetNext(date));
		}

		[Test]
		public void Should_be_able_to_satisfy_last_weekday()
		{
			var field = new CronDayOfMonth("LW");

			var date = new DateTime(2011, 01, 01, 0, 0, 0);

			Assert.AreEqual(date.AddDays(30), field.GetNext(date));
		}

		[Test]
		public void Should_be_able_to_satisfy_individual_values()
		{
			var field = new CronDayOfMonth("5,10,15,20");

			var control = new DateTime(2011, 01, 01, 0, 0, 0);
			var date = field.GetNext(control);

			Assert.AreEqual(control.AddDays(4), date);
			date = field.GetNext(date.AddDays(1));

			Assert.AreEqual(control.AddDays(9), date);
			date = field.GetNext(date.AddDays(1));

			Assert.AreEqual(control.AddDays(14), date);
			date = field.GetNext(date.AddDays(1));

			Assert.AreEqual(control.AddDays(19), date);
			date = field.GetNext(date.AddDays(1));

			Assert.AreEqual(control.AddDays(35), date);
		}

		[Test]
		public void Should_be_able_to_satisfy_a_range_of_values()
		{
			var field = new CronDayOfMonth("5-10");

			var control = new DateTime(2011, 01, 01, 0, 0, 0);
			var date = field.GetNext(control);

			Assert.AreEqual(control.AddDays(4), date);
			date = field.GetNext(date.AddDays(1));

			Assert.AreEqual(control.AddDays(5), date);
			date = field.GetNext(date.AddDays(1));

			Assert.AreEqual(control.AddDays(6), date);
			date = field.GetNext(date.AddDays(1));

			Assert.AreEqual(control.AddDays(7), date);
			date = field.GetNext(date.AddDays(1));

			Assert.AreEqual(control.AddDays(8), date);
			date = field.GetNext(date.AddDays(1));

			Assert.AreEqual(control.AddDays(9), date);
		}

		[Test]
		public void Should_be_able_to_satisfy_a_stepped_values()
		{
			var field = new CronDayOfMonth("5-10/5");

			var control = new DateTime(2011, 01, 01, 0, 0, 0);
			var date = field.GetNext(control);

			Assert.AreEqual(control.AddDays(4), date);
			date = field.GetNext(date.AddDays(1));

			Assert.AreEqual(control.AddDays(9), date);
			date = field.GetNext(date.AddDays(1));

			Assert.AreEqual(control.AddDays(35), date);
		}

		[Test]
		public void Should_throw_exceptions_on_invalid_expressions()
		{
			Assert.Throws<NullReferenceException>(() => new CronDayOfMonth(""));
			Assert.Throws<CronException>(() => new CronDayOfMonth("invalid"));
			Assert.Throws<CronException>(() => new CronDayOfMonth("10-60"));
			Assert.Throws<CronException>(() => new CronDayOfMonth("60-60"));
			Assert.Throws<CronException>(() => new CronDayOfMonth("10-5"));

			new CronDayOfMonth("*/15");
		}

        [Test]
        public void Should_fail_with_non_standard_character_when_specification_factory_does_not_support_it()
        {
			var factory = new SpecificationFactory(parameters =>
            {
                return !parameters.Expression.Equals("H", StringComparison.InvariantCultureIgnoreCase) 
                    ? null 
                    : new Specification<CronField.Candidate>(candidate => candidate.Date.Day % 2 == 0);
            });

            CronDayOfMonth field = null;
            var control = new DateTime(DateTime.Now.Year, 01, 01);
            var date = control.AddDays(-1);

			Assert.That(() => new CronDayOfMonth("I", factory), Throws.TypeOf<CronException>());
            Assert.That(() => field = new CronDayOfMonth("H", factory), Throws.Nothing);

            for (var i = 0; i < 15; i++)
            {
                date = field.GetNext(date.AddDays(1));

                Assert.That(date, Is.EqualTo(control.AddDays(i * 2 + 1)));
            }
		}
	}
}