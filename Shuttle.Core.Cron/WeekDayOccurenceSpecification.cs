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
        Guard.AgainstNull(item, nameof(item));

        return (int)item.Date.DayOfWeek + 1 == _weekDay && _occurrence == item.Date.Day / 7 + 1;
    }
}