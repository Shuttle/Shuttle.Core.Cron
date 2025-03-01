using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Specification;

namespace Shuttle.Core.Cron;

public class LastDayOfMonthSpecification : ISpecification<CronField.Candidate>
{
    public bool IsSatisfiedBy(CronField.Candidate item)
    {
        Guard.AgainstNull(item);

        return item.DateTimeOffset.Day == DateTime.DaysInMonth(item.DateTimeOffset.Year, item.DateTimeOffset.Month);
    }
}