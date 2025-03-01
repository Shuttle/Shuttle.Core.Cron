using System;
using NUnit.Framework;

namespace Shuttle.Core.Cron.Tests;

[TestFixture]
public class CronMonthTest
{
    [Test]
    public void Should_be_able_to_satisfy_asterisk_value()
    {
        var field = new CronMonth("*");

        var date = new DateTimeOffset(2011, 01, 01, 0, 0, 0, TimeSpan.Zero);

        for (var i = 1; i < 13; i++)
        {
            Assert.That(date, Is.EqualTo(field.GetNext(date)));

            date = date.AddHours(1);
        }
    }

    [Test]
    public void Should_be_able_to_satisfy_individual_values()
    {
        var field = new CronMonth("2,APR,May,8,oct");

        var control = new DateTimeOffset(2011, 01, 01, 0, 0, 0, TimeSpan.Zero);
        var date = field.GetNext(control);

        Assert.That(control.AddMonths(1), Is.EqualTo(date));
        date = field.GetNext(date.AddMonths(1));

        Assert.That(control.AddMonths(3), Is.EqualTo(date));
        date = field.GetNext(date.AddMonths(1));

        Assert.That(control.AddMonths(4), Is.EqualTo(date));
        date = field.GetNext(date.AddMonths(1));

        Assert.That(control.AddMonths(7), Is.EqualTo(date));
        date = field.GetNext(date.AddMonths(1));

        Assert.That(control.AddMonths(9), Is.EqualTo(date));
    }

    [Test]
    public void Should_be_able_to_satisfy_a_range_of_values()
    {
        var field = new CronMonth("5-10");

        var control = new DateTimeOffset(2011, 01, 01, 0, 0, 0, TimeSpan.Zero);
        var date = field.GetNext(control);

        Assert.That(control.AddMonths(4), Is.EqualTo(date));
        date = field.GetNext(date.AddMonths(1));

        Assert.That(control.AddMonths(5), Is.EqualTo(date));
        date = field.GetNext(date.AddMonths(1));

        Assert.That(control.AddMonths(6), Is.EqualTo(date));
        date = field.GetNext(date.AddMonths(1));

        Assert.That(control.AddMonths(7), Is.EqualTo(date));
        date = field.GetNext(date.AddMonths(1));

        Assert.That(control.AddMonths(8), Is.EqualTo(date));
        date = field.GetNext(date.AddMonths(1));

        Assert.That(control.AddMonths(9), Is.EqualTo(date));
    }

    [Test]
    public void Should_be_able_to_satisfy_a_stepped_values()
    {
        var field = new CronMonth("5-10/5");

        var control = new DateTimeOffset(2011, 01, 01, 0, 0, 0, TimeSpan.Zero);
        var date = field.GetNext(control);

        Assert.That(control.AddMonths(4), Is.EqualTo(date));
        date = field.GetNext(date.AddMonths(1));

        Assert.That(control.AddMonths(9), Is.EqualTo(date));
        date = field.GetNext(date.AddMonths(1));

        Assert.That(control.AddMonths(16), Is.EqualTo(date));
    }

    [Test]
    public void Should_throw_exceptions_on_invalid_expressions()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new CronMonth(""));
        Assert.Throws<CronException>(() => _ = new CronMonth("invalid"));
        Assert.Throws<CronException>(() => _ = new CronMonth("10-60"));
        Assert.Throws<CronException>(() => _ = new CronMonth("60-60"));
        Assert.Throws<CronException>(() => _ = new CronMonth("10-5"));

        _ = new CronMonth("*/15");
    }
}