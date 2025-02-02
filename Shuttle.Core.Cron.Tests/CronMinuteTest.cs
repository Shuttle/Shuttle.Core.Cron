using System;
using NUnit.Framework;

namespace Shuttle.Core.Cron.Tests;

[TestFixture]
public class CronMinuteTest
{
    [Test]
    public void Should_be_able_to_satisfy_asterisk_value()
    {
        var field = new CronMinute("*");

        var date = new DateTime(2011, 01, 01, 0, 0, 0);

        for (var i = 0; i < 60; i++)
        {
            Assert.That(date, Is.EqualTo(field.GetNext(date)));

            date = date.AddMinutes(1);
        }
    }

    [Test]
    public void Should_be_able_to_satisfy_individual_values()
    {
        var field = new CronMinute("5,10,15,30,45");

        var control = new DateTime(2011, 01, 01, 0, 0, 0);
        var date = field.GetNext(control);

        Assert.That(control.AddMinutes(5), Is.EqualTo(date));

        date = field.GetNext(date.AddMinutes(1));

        Assert.That(control.AddMinutes(10), Is.EqualTo(date));
        date = field.GetNext(date.AddMinutes(1));

        Assert.That(control.AddMinutes(15), Is.EqualTo(date));
        date = field.GetNext(date.AddMinutes(1));

        Assert.That(control.AddMinutes(30), Is.EqualTo(date));
        date = field.GetNext(date.AddMinutes(1));

        Assert.That(control.AddMinutes(45), Is.EqualTo(date));
        date = field.GetNext(date.AddMinutes(1));

        Assert.That(control.AddMinutes(65), Is.EqualTo(date));
    }

    [Test]
    public void Should_be_able_to_satisfy_a_range_of_values()
    {
        var field = new CronMinute("5-10");

        var control = new DateTime(2011, 01, 01, 0, 0, 0);
        var date = field.GetNext(control);

        Assert.That(control.AddMinutes(5), Is.EqualTo(date));
        date = field.GetNext(date.AddMinutes(1));

        Assert.That(control.AddMinutes(6), Is.EqualTo(date));
        date = field.GetNext(date.AddMinutes(1));

        Assert.That(control.AddMinutes(7), Is.EqualTo(date));
        date = field.GetNext(date.AddMinutes(1));

        Assert.That(control.AddMinutes(8), Is.EqualTo(date));
        date = field.GetNext(date.AddMinutes(1));

        Assert.That(control.AddMinutes(9), Is.EqualTo(date));
        date = field.GetNext(date.AddMinutes(1));

        Assert.That(control.AddMinutes(10), Is.EqualTo(date));
    }

    [Test]
    public void Should_be_able_to_satisfy_a_stepped_values()
    {
        var field = new CronMinute("5-10/5");

        var control = new DateTime(2011, 01, 01, 0, 0, 0);
        var date = field.GetNext(control);

        Assert.That(control.AddMinutes(5), Is.EqualTo(date));
        date = field.GetNext(date.AddMinutes(1));

        Assert.That(control.AddMinutes(10), Is.EqualTo(date));
        date = field.GetNext(date.AddMinutes(1));

        Assert.That(control.AddMinutes(65), Is.EqualTo(date));
    }

    [Test]
    public void Should_throw_exceptions_on_invalid_expressions()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new CronMinute(""));
        Assert.Throws<CronException>(() => _ = new CronMinute("invalid"));
        Assert.Throws<CronException>(() => _ = new CronMinute("10-60"));
        Assert.Throws<CronException>(() => _ = new CronMinute("60-60"));
        Assert.Throws<CronException>(() => _ = new CronMinute("10-5"));

        _ = new CronMinute("*/15");
    }
}