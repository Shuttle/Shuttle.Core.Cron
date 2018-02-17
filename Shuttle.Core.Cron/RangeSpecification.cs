using System.Collections.Generic;
using Shuttle.Core.Contract;
using Shuttle.Core.Specification;

namespace Shuttle.Core.Cron
{
	public class RangeSpecification : ISpecification<object>
	{
		private readonly List<int> _values = new List<int>();

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

		public bool IsSatisfiedBy(object item)
		{
			Guard.AgainstNull(item, nameof(item));

			if (item is int i)
			{
				return _values.Contains(i);
			}

			throw new CronException(string.Format(Resources.CronInvalidSpecificationCandidate, typeof(int).FullName, item.GetType().FullName));
		}
	}
}