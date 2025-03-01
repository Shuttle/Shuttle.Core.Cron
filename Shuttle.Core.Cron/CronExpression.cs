using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Cron;

public enum FieldName
{
    Minute = 0,
    Hour = 1,
    DayOfMonth = 2,
    Month = 3,
    DayOfWeek = 4
}

public class CronExpression
{
    private readonly long _minuteTicks = TimeSpan.FromMinutes(1).Ticks;
    private DateTimeOffset _cronDateTimeOffset;
    private readonly CronDayOfMonth _cronDayOfMonth;
    private readonly CronDayOfWeek _cronDayOfWeek;
    private readonly CronHour _cronHour;
    private readonly CronMinute _cronMinute;
    private readonly CronMonth _cronMonth;

    public CronExpression(string expression, ISpecificationFactory? specificationFactory = null)
        : this(expression, DateTimeOffset.UtcNow, specificationFactory)
    {
    }

    public CronExpression(string expression, DateTimeOffset dateTimeOffset, ISpecificationFactory? specificationFactory = null)
    {
        Expression = Guard.AgainstNullOrEmptyString(expression);

        _cronDateTimeOffset = Truncate(dateTimeOffset);

        var factory = specificationFactory ?? new SpecificationFactory();

        var values = expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        var length = values.Length;

        Guard.Against<CronException>(length != 5, string.Format(Resources.CronInvalidFieldCount, length));

        _cronMinute = new(values[0], factory);
        _cronHour = new(values[1], factory);
        _cronDayOfMonth = new(values[2], factory);
        _cronMonth = new(values[3], factory);
        _cronDayOfWeek = new(values[4], factory);

        Guard.Against<CronException>(_cronDayOfMonth.ExpressionType == ExpressionType.Skipped &&
                                     _cronDayOfWeek.ExpressionType == ExpressionType.Skipped, string.Format(Resources.CronNoDaysSpecified, expression));
        Guard.Against<CronException>(_cronDayOfMonth.ExpressionType != ExpressionType.Skipped &&
                                     _cronDayOfMonth.ExpressionType != ExpressionType.All &&
                                     _cronDayOfWeek.ExpressionType != ExpressionType.Skipped &&
                                     _cronDayOfWeek.ExpressionType != ExpressionType.All, string.Format(Resources.CronBothDaysSpecified, expression));
    }

    public string Expression { get; }

    public DateTimeOffset GetNextOccurrence(DateTimeOffset dateTimeOffset)
    {
        var result = Truncate(dateTimeOffset);

        result = _cronMinute.GetNext(result);
        result = _cronHour.GetNext(result);
        result = _cronDayOfMonth.GetNext(result);
        result = _cronMonth.GetNext(result);
        result = _cronDayOfWeek.GetNext(result);

        return result;
    }

    public DateTimeOffset GetPreviousOccurrence(DateTimeOffset dateTimeOffset)
    {
        var result = Truncate(dateTimeOffset);

        result = _cronMinute.GetPrevious(result);
        result = _cronHour.GetPrevious(result);
        result = _cronDayOfMonth.GetPrevious(result);
        result = _cronMonth.GetPrevious(result);
        result = _cronDayOfWeek.GetPrevious(result);

        return result;
    }

    public DateTimeOffset NextOccurrence()
    {
        return NextOccurrence(_cronDateTimeOffset);
    }

    public DateTimeOffset NextOccurrence(DateTimeOffset dateTimeOffset)
    {
        _cronDateTimeOffset = GetNextOccurrence(Truncate(dateTimeOffset.AddMinutes(1)));

        var validator = GetNextOccurrence(_cronDateTimeOffset);

        while (validator != _cronDateTimeOffset)
        {
            _cronDateTimeOffset = validator;
            validator = GetNextOccurrence(_cronDateTimeOffset);
        }

        return _cronDateTimeOffset;
    }

    public DateTimeOffset PreviousOccurrence()
    {
        return PreviousOccurrence(_cronDateTimeOffset);
    }

    public DateTimeOffset PreviousOccurrence(DateTimeOffset dateTimeOffset)
    {
        _cronDateTimeOffset = GetPreviousOccurrence(Truncate(dateTimeOffset.AddMinutes(-1)));

        var validator = GetPreviousOccurrence(_cronDateTimeOffset);

        while (validator != _cronDateTimeOffset)
        {
            _cronDateTimeOffset = validator;
            validator = GetPreviousOccurrence(_cronDateTimeOffset);
        }

        return _cronDateTimeOffset;
    }

    private DateTimeOffset Truncate(DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.AddTicks(-(dateTimeOffset.Ticks % _minuteTicks));
    }
}