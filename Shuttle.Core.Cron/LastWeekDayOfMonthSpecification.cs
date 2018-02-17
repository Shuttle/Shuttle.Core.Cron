using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Specification;

namespace Shuttle.Core.Cron
{
	public class LastWeekDayOfMonthSpecification : ISpecification<object>
	{
		public bool IsSatisfiedBy(object item)
		{
			Guard.AgainstNull(item, nameof(item));

			if (item is DateTime date)
			{
			    var compare = date.AddDays(date.Day*-1);

				for (var day = DateTime.DaysInMonth(date.Year, date.Month); day > 0; day--)
				{
					var dayOfWeek = compare.AddDays(day).DayOfWeek;

					if (dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday)
					{
						return date.Day == day;
					}
				}
			}

			throw new CronException(string.Format(Resources.CronInvalidSpecificationCandidate, typeof(int).FullName, item.GetType().FullName));
		}
	}
}