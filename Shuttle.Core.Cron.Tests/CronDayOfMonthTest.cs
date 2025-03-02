using System;
using NUnit.Framework;
using Shuttle.Core.Specification;

namespace Shuttle.Core.Cron.Tests;

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
            Assert.That(field.GetNext(date), Is.EqualTo(date));

            date = date.AddDays(1);
        }
    }

    [Test]
    public void Should_be_able_to_satisfy_last_day_of_month()
    {
        var field = new CronDayOfMonth("L");

        var date = new DateTime(2011, 01, 01, 0, 0, 0);

        Assert.That(field.GetNext(date), Is.EqualTo(date.AddDays(30)));
    }

    [Test]
    public void Should_be_able_to_satisfy_absolute_weekday()
    {
        var field = new CronDayOfMonth("12W");

        var date = new DateTime(2011, 01, 01, 0, 0, 0);

        Assert.That(field.GetNext(date), Is.EqualTo(date.AddDays(11)));
    }

    [Test]
    public void Should_be_able_to_satisfy_mid_month_saturday_weekday()
    {
        var field = new CronDayOfMonth("15W");

        var date = new DateTime(2011, 01, 01, 0, 0, 0);

        Assert.That(field.GetNext(date), Is.EqualTo(date.AddDays(13)));
    }

    [Test]
    public void Should_be_able_to_satisfy_mid_month_sunday_weekday()
    {
        var field = new CronDayOfMonth("16W");

        var date = new DateTime(2011, 01, 01, 0, 0, 0);

        Assert.That(field.GetNext(date), Is.EqualTo(date.AddDays(16)));
    }

    [Test]
    public void Should_be_able_to_satisfy_first_day_saturday_weekday()
    {
        var field = new CronDayOfMonth("1W");

        var date = new DateTime(2011, 01, 01, 0, 0, 0);

        Assert.That(field.GetNext(date), Is.EqualTo(date.AddDays(2)));
    }

    [Test]
    public void Should_be_able_to_satisfy_last_day_sunday_weekday()
    {
        var field = new CronDayOfMonth("31W");

        var date = new DateTime(2011, 07, 01, 0, 0, 0);

        Assert.That(field.GetNext(date), Is.EqualTo(date.AddDays(28)));
    }

    [Test]
    public void Should_be_able_to_satisfy_last_weekday()
    {
        var field = new CronDayOfMonth("LW");

        var date = new DateTime(2011, 01, 01, 0, 0, 0);

        Assert.That(field.GetNext(date), Is.EqualTo(date.AddDays(30)));
    }

    [Test]
    public void Should_be_able_to_satisfy_individual_values()
    {
        var field = new CronDayOfMonth("5,10,15,20");

        var control = new DateTime(2011, 01, 01, 0, 0, 0);
        var date = field.GetNext(control);

        Assert.That(date, Is.EqualTo(control.AddDays(4)));
        date = field.GetNext(date.AddDays(1));

        Assert.That(date, Is.EqualTo(control.AddDays(9)));
        date = field.GetNext(date.AddDays(1));

        Assert.That(date, Is.EqualTo(control.AddDays(14)));
        date = field.GetNext(date.AddDays(1));

        Assert.That(date, Is.EqualTo(control.AddDays(19)));
        date = field.GetNext(date.AddDays(1));

        Assert.That(date, Is.EqualTo(control.AddDays(35)));
    }

    [Test]
    public void Should_be_able_to_satisfy_a_range_of_values()
    {
        var field = new CronDayOfMonth("5-10");

        var control = new DateTime(2011, 01, 01, 0, 0, 0);
        var date = field.GetNext(control);

        Assert.That(date, Is.EqualTo(control.AddDays(4)));
        date = field.GetNext(date.AddDays(1));

        Assert.That(date, Is.EqualTo(control.AddDays(5)));
        date = field.GetNext(date.AddDays(1));

        Assert.That(date, Is.EqualTo(control.AddDays(6)));
        date = field.GetNext(date.AddDays(1));

        Assert.That(date, Is.EqualTo(control.AddDays(7)));
        date = field.GetNext(date.AddDays(1));

        Assert.That(date, Is.EqualTo(control.AddDays(8)));
        date = field.GetNext(date.AddDays(1));

        Assert.That(date, Is.EqualTo(control.AddDays(9)));
    }

    [Test]
    public void Should_be_able_to_satisfy_a_stepped_values()
    {
        var field = new CronDayOfMonth("5-10/5");

        var control = new DateTime(2011, 01, 01, 0, 0, 0);
        var date = field.GetNext(control);

        Assert.That(date, Is.EqualTo(control.AddDays(4)));
        date = field.GetNext(date.AddDays(1));

        Assert.That(date, Is.EqualTo(control.AddDays(9)));
        date = field.GetNext(date.AddDays(1));

        Assert.That(date, Is.EqualTo(control.AddDays(35)));
    }

    [Test]
    public void Should_throw_exceptions_on_invalid_expressions()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new CronDayOfMonth(""));
        Assert.Throws<CronException>(() => _ = new CronDayOfMonth("invalid"));
        Assert.Throws<CronException>(() => _ = new CronDayOfMonth("10-60"));
        Assert.Throws<CronException>(() => _ = new CronDayOfMonth("60-60"));
        Assert.Throws<CronException>(() => _ = new CronDayOfMonth("10-5"));

        _ = new CronDayOfMonth("*/15");
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
        Assert.That(() => field = new("H", factory), Throws.Nothing);

        for (var i = 0; i < 15; i++)
        {
            date = field.GetNext(date.AddDays(1));

            Assert.That(date, Is.EqualTo(control.AddDays(i * 2 + 1)));
        }
    }
}