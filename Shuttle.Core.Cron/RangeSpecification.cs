using System.Collections.Generic;
using Shuttle.Core.Contract;
using Shuttle.Core.Specification;

namespace Shuttle.Core.Cron;

public class RangeSpecification : ISpecification<CronField.Candidate>
{
    private readonly List<int> _values = new();

    public RangeSpecification(int start, int end, int step)
    {
        Guard.Against<CronException>(end < start, Resources.CronStartValueLargerThanEndValue);

        var value = start;

        while (value <= end)
        {
            _values.Add(value);

            value += step;
        }
    }

    public RangeSpecification(int value)
        : this(value, value, 1)
    {
    }

    public bool IsSatisfiedBy(CronField.Candidate item)
    {
        Guard.AgainstNull(item);

        int compare;

        switch (item.FieldName)
        {
            case FieldName.DayOfWeek:
            {
                compare = (int)item.DateTimeOffset.DayOfWeek + 1;
                break;
            }
            case FieldName.DayOfMonth:
            {
                compare = item.DateTimeOffset.Day;
                break;
            }
            case FieldName.Month:
            {
                compare = item.DateTimeOffset.Month;
                break;
            }
            case FieldName.Hour:
            {
                compare = item.DateTimeOffset.Hour;
                break;
            }
            case FieldName.Minute:
            {
                compare = item.DateTimeOffset.Minute;
                break;
            }
            default:
            {
                throw new CronException(string.Format(Resources.CronInvalidFieldNameExcaption, typeof(int).FullName, item.GetType().FullName));
            }
        }

        return _values.Contains(compare);
    }
}