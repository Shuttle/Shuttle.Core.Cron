using System;
using NUnit.Framework;

namespace Shuttle.Core.Cron.Tests;

[TestFixture]
public class CronExpressionTest
{
    [Test]
    public void Should_be_able_to_create_an_all_inclusive_expression()
    {
        var date = new DateTimeOffset(2011, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var cron = new CronExpression("* * * * *", date);

        for (var i = 1; i < 121; i++)
        {
            Assert.That(date.AddMinutes(i), Is.EqualTo(cron.NextOccurrence()));
        }
    }

    [Test]
    public void Should_be_able_to_get_every_five_minutes()
    {
        var date = new DateTimeOffset(2011, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var cron = new CronExpression("0/5 * * * *", date);

        for (var i = 1; i < 21; i++)
        {
            Assert.That(cron.NextOccurrence(), Is.EqualTo(date.AddMinutes(5 * i)));
        }
    }

    [Test]
    public void Should_be_able_to_get_individual_minutes()
    {
        var date = new DateTimeOffset(2011, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var cron = new CronExpression("5,10,15,30,45 * * * *", date);

        Assert.That(cron.NextOccurrence(), Is.EqualTo(date.AddMinutes(5)));
        Assert.That(cron.NextOccurrence(), Is.EqualTo(date.AddMinutes(10)));
        Assert.That(cron.NextOccurrence(), Is.EqualTo(date.AddMinutes(15)));
        Assert.That(cron.NextOccurrence(), Is.EqualTo(date.AddMinutes(30)));
        Assert.That(cron.NextOccurrence(), Is.EqualTo(date.AddMinutes(45)));
    }

    [Test]
    public void Should_be_able_to_get_last_day_of_month_across_multiple_months()
    {
        var date = new DateTimeOffset(2011, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var cron = new CronExpression("0 0 L 1,2,5,6,9,12 *", date);

        Assert.That(new DateTimeOffset(2011, 1, 31, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.NextOccurrence()));
        Assert.That(new DateTimeOffset(2011, 2, 28, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.NextOccurrence()));
        Assert.That(new DateTimeOffset(2011, 5, 31, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.NextOccurrence()));
        Assert.That(new DateTimeOffset(2011, 6, 30, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.NextOccurrence()));
        Assert.That(new DateTimeOffset(2011, 9, 30, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.NextOccurrence()));
        Assert.That(new DateTimeOffset(2011, 12, 31, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.NextOccurrence()));

        date = new(2012, 1, 1, 0, 0, 0, TimeSpan.Zero);
        cron = new("0 0 L 1,2,5,6,9,12 *", date);

        Assert.That(new DateTimeOffset(2011, 12, 31, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.PreviousOccurrence()));
        Assert.That(new DateTimeOffset(2011, 9, 30, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.PreviousOccurrence()));
        Assert.That(new DateTimeOffset(2011, 6, 30, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.PreviousOccurrence()));
        Assert.That(new DateTimeOffset(2011, 5, 31, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.PreviousOccurrence()));
        Assert.That(new DateTimeOffset(2011, 2, 28, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.PreviousOccurrence()));
        Assert.That(new DateTimeOffset(2011, 1, 31, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.PreviousOccurrence()));
    }

    [Test]
    public void Should_be_able_to_get_next_occurrence()
    {
        var dateTime = new DateTimeOffset(2000, 1, 1, 1, 0, 3, TimeSpan.Zero);
        var cron = new CronExpression("0 2 * * *", dateTime);

        var previous1 = cron.GetNextOccurrence(dateTime);
        var previous2 = cron.NextOccurrence();

        Assert.That(previous2, Is.EqualTo(previous1));
    }

    [Test]
    public void Should_be_able_to_get_previous_occurrence()
    {
        var dateTime = new DateTimeOffset(2000, 1, 1, 1, 0, 3, TimeSpan.Zero);
        var cron = new CronExpression("0 2 * * *", dateTime);

        var previous1 = cron.GetPreviousOccurrence(dateTime);
        var previous2 = cron.PreviousOccurrence();

        Assert.That(previous2, Is.EqualTo(previous1));
    }

    [Test]
    public void Should_be_able_to_get_week_days_across_month_boundary()
    {
        var date = new DateTimeOffset(2011, 1, 23, 0, 0, 0, TimeSpan.Zero);
        var cron = new CronExpression("0 0 ? * 3,6", date);

        Assert.That(new DateTimeOffset(2011, 1, 25, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.NextOccurrence()));
        Assert.That(new DateTimeOffset(2011, 1, 28, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.NextOccurrence()));
        Assert.That(new DateTimeOffset(2011, 2, 1, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.NextOccurrence()));
        Assert.That(new DateTimeOffset(2011, 2, 4, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.NextOccurrence()));
        Assert.That(new DateTimeOffset(2011, 2, 8, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.NextOccurrence()));

        date = new(2011, 2, 10, 0, 0, 0, TimeSpan.Zero);
        cron = new("0 0 ? * 3,6", date);

        Assert.That(new DateTimeOffset(2011, 2, 8, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.PreviousOccurrence()));
        Assert.That(new DateTimeOffset(2011, 2, 4, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.PreviousOccurrence()));
        Assert.That(new DateTimeOffset(2011, 2, 1, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.PreviousOccurrence()));
        Assert.That(new DateTimeOffset(2011, 1, 28, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.PreviousOccurrence()));
        Assert.That(new DateTimeOffset(2011, 1, 25, 0, 0, 0, TimeSpan.Zero), Is.EqualTo(cron.PreviousOccurrence()));
    }

    [Test]
    public void Should_throw_exceptions_for_invalid_expressions()
    {
        Assert.Throws<CronException>(() => _ = new CronExpression("* * *"));
        Assert.Throws<CronException>(() => _ = new CronExpression("* * * * * *"));
        Assert.Throws<CronException>(() => _ = new CronExpression("30-20 * * * * *"));
        Assert.Throws<CronException>(() => _ = new CronExpression("* 1-24 * * * *"));
        Assert.Throws<CronException>(() => _ = new CronExpression("* * X * *"));
        Assert.Throws<CronException>(() => _ = new CronExpression("* * X * *"));
        Assert.Throws<CronException>(() => _ = new CronExpression("* * ? * ?"));
        Assert.Throws<CronException>(() => _ = new CronExpression("* * 1 * 1"));
    }
}