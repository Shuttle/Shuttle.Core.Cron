using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Specification;

namespace Shuttle.Core.Cron;

public class LastWeekDayOfMonthSpecification : ISpecification<CronField.Candidate>
{
    public bool IsSatisfiedBy(CronField.Candidate item)
    {
        Guard.AgainstNull(item);

        var compare = item.DateTimeOffset.AddDays(item.DateTimeOffset.Day * -1);

        for (var day = DateTime.DaysInMonth(item.DateTimeOffset.Year, item.DateTimeOffset.Month); day > 0; day--)
        {
            var dayOfWeek = compare.AddDays(day).DayOfWeek;

            if (dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday)
            {
                return item.DateTimeOffset.Day == day;
            }
        }

        return false;
    }
}