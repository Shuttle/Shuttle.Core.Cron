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
    private DateTime _cronDate;
    private readonly CronDayOfMonth _cronDayOfMonth;
    private readonly CronDayOfWeek _cronDayOfWeek;
    private readonly CronHour _cronHour;
    private readonly CronMinute _cronMinute;
    private readonly CronMonth _cronMonth;

    public CronExpression(string expression, ISpecificationFactory? specificationFactory = null)
        : this(expression, DateTime.Now, specificationFactory)
    {
    }

    public CronExpression(string expression, DateTime date, ISpecificationFactory? specificationFactory = null)
    {
        Expression = Guard.AgainstNullOrEmptyString(expression);

        _cronDate = Truncate(date);

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

    public DateTime GetNextOccurrence(DateTime date)
    {
        var result = Truncate(date);

        result = _cronMinute.GetNext(result);
        result = _cronHour.GetNext(result);
        result = _cronDayOfMonth.GetNext(result);
        result = _cronMonth.GetNext(result);
        result = _cronDayOfWeek.GetNext(result);

        return result;
    }

    public DateTime GetPreviousOccurrence(DateTime date)
    {
        var result = Truncate(date);

        result = _cronMinute.GetPrevious(result);
        result = _cronHour.GetPrevious(result);
        result = _cronDayOfMonth.GetPrevious(result);
        result = _cronMonth.GetPrevious(result);
        result = _cronDayOfWeek.GetPrevious(result);

        return result;
    }

    public DateTime NextOccurrence()
    {
        return NextOccurrence(_cronDate);
    }

    public DateTime NextOccurrence(DateTime date)
    {
        _cronDate = GetNextOccurrence(Truncate(date.AddMinutes(1)));

        var validator = GetNextOccurrence(_cronDate);

        while (validator != _cronDate)
        {
            _cronDate = validator;
            validator = GetNextOccurrence(_cronDate);
        }

        return _cronDate;
    }

    public DateTime PreviousOccurrence()
    {
        return PreviousOccurrence(_cronDate);
    }

    public DateTime PreviousOccurrence(DateTime date)
    {
        _cronDate = GetPreviousOccurrence(Truncate(date.AddMinutes(-1)));

        var validator = GetPreviousOccurrence(_cronDate);

        while (validator != _cronDate)
        {
            _cronDate = validator;
            validator = GetPreviousOccurrence(_cronDate);
        }

        return _cronDate;
    }

    private DateTime Truncate(DateTime dateTime)
    {
        return dateTime.AddTicks(-(dateTime.Ticks % _minuteTicks));
    }
}