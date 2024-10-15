using System;
using NUnit.Framework;

namespace Shuttle.Core.Cron.Tests;

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
            Assert.That(date, Is.EqualTo(field.GetNext(date)));

            date = date.AddHours(1);
        }
    }

    [Test]
    public void Should_be_able_to_satisfy_individual_values()
    {
        var field = new CronHour("5,10,15,20");

        var control = new DateTime(2011, 01, 01, 0, 0, 0);
        var date = field.GetNext(control);

        Assert.That(control.AddHours(5), Is.EqualTo(date));
        date = field.GetNext(date.AddHours(1));

        Assert.That(control.AddHours(10), Is.EqualTo(date));
        date = field.GetNext(date.AddHours(1));

        Assert.That(control.AddHours(15), Is.EqualTo(date));
        date = field.GetNext(date.AddHours(1));

        Assert.That(control.AddHours(20), Is.EqualTo(date));
        date = field.GetNext(date.AddHours(1));

        Assert.That(control.AddHours(29), Is.EqualTo(date));
    }

    [Test]
    public void Should_be_able_to_satisfy_a_range_of_values()
    {
        var field = new CronHour("5-10");

        var control = new DateTime(2011, 01, 01, 0, 0, 0);
        var date = field.GetNext(control);

        Assert.That(control.AddHours(5), Is.EqualTo(date));
        date = field.GetNext(date.AddHours(1));

        Assert.That(control.AddHours(6), Is.EqualTo(date));
        date = field.GetNext(date.AddHours(1));

        Assert.That(control.AddHours(7), Is.EqualTo(date));
        date = field.GetNext(date.AddHours(1));

        Assert.That(control.AddHours(8), Is.EqualTo(date));
        date = field.GetNext(date.AddHours(1));

        Assert.That(control.AddHours(9), Is.EqualTo(date));
        date = field.GetNext(date.AddHours(1));

        Assert.That(control.AddHours(10), Is.EqualTo(date));
    }

    [Test]
    public void Should_be_able_to_satisfy_a_stepped_values()
    {
        var field = new CronHour("5-10/5");

        var control = new DateTime(2011, 01, 01, 0, 0, 0);
        var date = field.GetNext(control);

        Assert.That(control.AddHours(5), Is.EqualTo(date));
        date = field.GetNext(date.AddHours(1));

        Assert.That(control.AddHours(10), Is.EqualTo(date));
        date = field.GetNext(date.AddHours(1));

        Assert.That(control.AddHours(29), Is.EqualTo(date));
    }

    [Test]
    public void Should_throw_exceptions_on_invalid_expressions()
    {
        Assert.Throws<NullReferenceException>(() => _ = new CronHour(""));
        Assert.Throws<CronException>(() => _ = new CronHour("invalid"));
        Assert.Throws<CronException>(() => _ = new CronHour("10-60"));
        Assert.Throws<CronException>(() => _ = new CronHour("60-60"));
        Assert.Throws<CronException>(() => _ = new CronHour("10-5"));

        _ = new CronHour("*/15");
    }
}