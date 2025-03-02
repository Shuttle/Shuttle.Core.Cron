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

        var firstDayOfWeek = new DateTime(item.Date.Year, item.Date.Month, 1).DayOfWeek;
        var daysOffset = (int)item.Date.DayOfWeek - (int)firstDayOfWeek;
        
        if (daysOffset < 0)
        {
            daysOffset += 7;
        }

        return (int)item.Date.DayOfWeek + 1 == _weekDay && 
               _occurrence == (item.Date.Day - (1 + daysOffset)) / 7 + 1;
    }
}