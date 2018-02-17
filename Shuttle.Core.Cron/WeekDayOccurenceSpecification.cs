using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Specification;

namespace Shuttle.Core.Cron
{
    public class WeekDayOccurenceSpecification : ISpecification<object>
    {
        private readonly int _occurrence;
        private readonly int _weekDay;

        public WeekDayOccurenceSpecification(int weekDay, int occurrence)
        {
            _weekDay = weekDay;
            _occurrence = occurrence;
        }

        public bool IsSatisfiedBy(object item)
        {
            Guard.AgainstNull(item, nameof(item));

            if (!(item is DateTime date))
            {
                throw new CronException(string.Format(Resources.CronInvalidSpecificationCandidate,
                    typeof(int).FullName, item.GetType().FullName));
            }

            var day = (int) date.DayOfWeek + 1;

            return day == _weekDay && _occurrence == date.Day / 7 + 1;

        }
    }
}