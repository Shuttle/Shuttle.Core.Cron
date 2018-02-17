using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Specification;

namespace Shuttle.Core.Cron
{
    public class LastDayOfMonthSpecification : ISpecification<object>
    {
        public bool IsSatisfiedBy(object item)
        {
            Guard.AgainstNull(item, nameof(item));

            if (item is DateTime date)
            {
                return date.Day == DateTime.DaysInMonth(date.Year, date.Month);
            }

            throw new CronException(string.Format(Resources.CronInvalidSpecificationCandidate, typeof(int).FullName,
                item.GetType().FullName));
        }
    }
}