using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Specification;

namespace Shuttle.Core.Cron;

public class WeekDayOccurrenceSpecification : ISpecification<CronField.Candidate>
{
    private readonly int _occurrence;
    private readonly int _weekDay;

    public WeekDayOccurrenceSpecification(int weekDay, int occurrence)
    {
        _weekDay = weekDay;
        _occurrence = occurrence;
    }

    public bool IsSatisfiedBy(CronField.Candidate item)
    {
        Guard.AgainstNull(item);

        var firstDayOfWeek = new DateTimeOffset(item.DateTimeOffset.Year, item.DateTimeOffset.Month, 1, 0, 0, 0, TimeSpan.Zero).DayOfWeek;
        var daysOffset = (int)item.DateTimeOffset.DayOfWeek - (int)firstDayOfWeek;
        
        if (daysOffset < 0)
        {
            daysOffset += 7;
        }

        return (int)item.DateTimeOffset.DayOfWeek + 1 == _weekDay && 
               _occurrence == (item.DateTimeOffset.Day - (1 + daysOffset)) / 7 + 1;
    }
}