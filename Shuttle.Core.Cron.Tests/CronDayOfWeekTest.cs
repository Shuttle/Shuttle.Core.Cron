using System;
using NUnit.Framework;

namespace Shuttle.Core.Cron.Tests;

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
            Assert.That(field.GetNext(date), Is.EqualTo(date));

            date = date.AddDays(1);
        }
    }

    [Test]
    public void Should_be_able_to_satisfy_last_day_of_week()
    {
        var field = new CronDayOfWeek("L");

        var control = new DateTime(2011, 01, 01, 0, 0, 0);
        var date = field.GetNext(control);

        Assert.That(control, Is.EqualTo(date));
        date = field.GetNext(date.AddDays(1));

        Assert.That(control.AddDays(7), Is.EqualTo(date));
        date = field.GetNext(date.AddDays(1));

        Assert.That(control.AddDays(14), Is.EqualTo(date));
        date = field.GetNext(date.AddDays(1));

        Assert.That(control.AddDays(21), Is.EqualTo(date));
        date = field.GetNext(date.AddDays(1));

        Assert.That(control.AddDays(28), Is.EqualTo(date));
    }

    [Test]
    public void Should_be_able_to_satisfy_occurrence()
    {
        var field = new CronDayOfWeek("6#1");

        var date = new DateTime(2011, 1, 1);

        Assert.That(field.GetNext(date), Is.EqualTo(date.AddDays(6)));

        field = new("6#2");

        Assert.That(field.GetNext(date), Is.EqualTo(date.AddDays(13)));

        field = new("6#3");

        Assert.That(field.GetNext(date), Is.EqualTo(date.AddDays(20)));

        field = new("6#4");

        Assert.That(field.GetNext(date), Is.EqualTo(date.AddDays(27)));
    }

    [Test]
    public void Should_be_able_to_satisfy_individual_values()
    {
        var field = new CronDayOfWeek("1,3,6");

        var control = new DateTime(2011, 01, 01, 0, 0, 0);
        var date = field.GetNext(control);

        Assert.That(control.AddDays(1), Is.EqualTo(date));
        date = field.GetNext(date.AddDays(1));

        Assert.That(control.AddDays(3), Is.EqualTo(date));
        date = field.GetNext(date.AddDays(1));

        Assert.That(control.AddDays(6), Is.EqualTo(date));
        date = field.GetNext(date.AddDays(1));

        Assert.That(control.AddDays(8), Is.EqualTo(date));
        date = field.GetNext(date.AddDays(1));

        Assert.That(control.AddDays(10), Is.EqualTo(date));
        date = field.GetNext(date.AddDays(1));

        Assert.That(control.AddDays(13), Is.EqualTo(date));
    }

    [Test]
    public void Should_be_able_to_satisfy_a_range_of_values()
    {
        var field = new CronDayOfWeek("2-4");

        var control = new DateTime(2011, 01, 01, 0, 0, 0);
        var date = field.GetNext(control);

        Assert.That(control.AddDays(2), Is.EqualTo(date));
        date = field.GetNext(date.AddDays(1));

        Assert.That(control.AddDays(3), Is.EqualTo(date));
        date = field.GetNext(date.AddDays(1));

        Assert.That(control.AddDays(4), Is.EqualTo(date));
        date = field.GetNext(date.AddDays(1));

        Assert.That(control.AddDays(9), Is.EqualTo(date));
        date = field.GetNext(date.AddDays(1));

        Assert.That(control.AddDays(10), Is.EqualTo(date));
        date = field.GetNext(date.AddDays(1));

        Assert.That(control.AddDays(11), Is.EqualTo(date));
    }

    [Test]
    public void Should_be_able_to_satisfy_a_stepped_values()
    {
        var field = new CronDayOfWeek("2/5");

        var control = new DateTime(2011, 01, 01, 0, 0, 0);
        var date = field.GetNext(control);

        Assert.That(control, Is.EqualTo(date));
        date = field.GetNext(date.AddDays(1));

        Assert.That(control.AddDays(2), Is.EqualTo(date));
        date = field.GetNext(date.AddDays(1));

        Assert.That(control.AddDays(7), Is.EqualTo(date));
        date = field.GetNext(date.AddDays(1));

        Assert.That(control.AddDays(9), Is.EqualTo(date));
        date = field.GetNext(date.AddDays(1));

        Assert.That(control.AddDays(14), Is.EqualTo(date));
    }

    [Test]
    public void Should_throw_exceptions_on_invalid_expressions()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new CronDayOfWeek(""));
        Assert.Throws<CronException>(() => _ = new CronDayOfWeek("invalid"));
        Assert.Throws<CronException>(() => _ = new CronDayOfWeek("10-60"));
        Assert.Throws<CronException>(() => _ = new CronDayOfWeek("60-60"));
        Assert.Throws<CronException>(() => _ = new CronDayOfWeek("10-5"));

        _ = new CronDayOfWeek("*/15");
    }
}