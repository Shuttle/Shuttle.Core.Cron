using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Specification;

namespace Shuttle.Core.Cron
{
    public class LastDayOfMonthSpecification : ISpecification<CronField.Candidate>
    {
        public bool IsSatisfiedBy(CronField.Candidate item)
        {
            Guard.AgainstNull(item, nameof(item));

            return item.Date.Day == DateTime.DaysInMonth(item.Date.Year, item.Date.Month);
        }
    }
}