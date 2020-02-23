using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Cron
{
    public enum FieldName
    {
        Minute = 0,
        Hour = 1,
        DayOfMonth = 2,
        Month = 3,
        DayOfWeek = 4
    }

    public class CronExpression
    {
        private readonly ISpecificationFactory _specificationFactory;
        private readonly long _minuteTicks = TimeSpan.FromMinutes(1).Ticks;
        private DateTime _cronDate;
        private CronDayOfMonth _cronDayOfMonth;
        private CronDayOfWeek _cronDayOfWeek;
        private CronHour _cronHour;
        private CronMinute _cronMinute;
        private CronMonth _cronMonth;

        public CronExpression(string expression, ISpecificationFactory specificationFactory = null)
            : this(expression, DateTime.Now, specificationFactory)
        {
        }

        public CronExpression(string expression, DateTime date, ISpecificationFactory specificationFactory = null)
        {
            Guard.AgainstNullOrEmptyString(expression, nameof(expression));

            Expression = expression;

            _cronDate = Truncate(date);
            _specificationFactory = specificationFactory ?? new DefaultSpecificationFactory();

            ParseExpression(expression);
        }

        public string Expression { get; }

        private DateTime Truncate(DateTime dateTime)
        {
            return dateTime.AddTicks(-(dateTime.Ticks % _minuteTicks));
        }

        private void ParseExpression(string expression)
        {
            var values = expression.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            var length = values.Length;

            Guard.Against<CronException>(length != 5, string.Format(Resources.CronInvalidFieldCount, length));

            _cronMinute = new CronMinute(values[0], _specificationFactory);
            _cronHour = new CronHour(values[1], _specificationFactory);
            _cronDayOfMonth = new CronDayOfMonth(values[2], _specificationFactory);
            _cronMonth = new CronMonth(values[3], _specificationFactory);
            _cronDayOfWeek = new CronDayOfWeek(values[4], _specificationFactory);

            Guard.Against<CronException>(_cronDayOfMonth.ExpressionType == ExpressionType.Skipped
                                         &&
                                         _cronDayOfWeek.ExpressionType == ExpressionType.Skipped,
                string.Format(Resources.CronNoDaysSpecified, expression));
            Guard.Against<CronException>(_cronDayOfMonth.ExpressionType != ExpressionType.Skipped
                                         &&
                                         _cronDayOfMonth.ExpressionType != ExpressionType.All
                                         &&
                                         _cronDayOfWeek.ExpressionType != ExpressionType.Skipped
                                         &&
                                         _cronDayOfWeek.ExpressionType != ExpressionType.All,
                string.Format(Resources.CronBothDaysSpecified, expression));
        }

        public DateTime NextOccurrence()
        {
            return NextOccurrence(_cronDate);
        }

        public DateTime NextOccurrence(DateTime date)
        {
            _cronDate = GetNextOccurrence(Truncate(date.AddMinutes(1)));

            var validator = GetNextOccurrence(_cronDate);

            while (validator != _cronDate)
            {
                _cronDate = validator;
                validator = GetNextOccurrence(_cronDate);
            }

            return _cronDate;
        }

        public DateTime GetNextOccurrence(DateTime date)
        {
            var result = date;

            result = _cronMinute.GetNext(result);
            result = _cronHour.GetNext(result);
            result = _cronDayOfMonth.GetNext(result);
            result = _cronMonth.GetNext(result);
            result = _cronDayOfWeek.GetNext(result);

            return result;
        }

        public DateTime PreviousOccurrence()
        {
            return PreviousOccurrence(_cronDate);
        }

        public DateTime PreviousOccurrence(DateTime date)
        {
            _cronDate = GetPreviousOccurrence(Truncate(date.AddMinutes(-1)));

            var validator = GetPreviousOccurrence(_cronDate);

            while (validator != _cronDate)
            {
                _cronDate = validator;
                validator = GetPreviousOccurrence(_cronDate);
            }

            return _cronDate;
        }

        public DateTime GetPreviousOccurrence(DateTime date)
        {
            var result = date;

            result = _cronMinute.GetPrevious(result);
            result = _cronHour.GetPrevious(result);
            result = _cronDayOfMonth.GetPrevious(result);
            result = _cronMonth.GetPrevious(result);
            result = _cronDayOfWeek.GetPrevious(result);

            return result;
        }
    }
}
