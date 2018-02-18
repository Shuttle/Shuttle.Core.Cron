using System;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Cron
{
    public class CronExpression
    {
        private CronDayOfMonth _cronDayOfMonth;
        private CronDayOfWeek _cronDayOfWeek;
        private CronHour _cronHour;
        private CronMinute _cronMinute;
        private CronMonth _cronMonth;
        private DateTime _crondate;
        private readonly long _minuteTicks = TimeSpan.FromMinutes(1).Ticks;

        public CronExpression(string expression)
            : this(expression, DateTime.Now)
        {
        }

        public CronExpression(string expression, DateTime date)
        {
            Guard.AgainstNullOrEmptyString(expression, nameof(expression));

            Expression = expression;

            _crondate = Truncate(date);

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

            _cronMinute = new CronMinute(values[0]);
            _cronHour = new CronHour(values[1]);
            _cronDayOfMonth = new CronDayOfMonth(values[2]);
            _cronMonth = new CronMonth(values[3]);
            _cronDayOfWeek = new CronDayOfWeek(values[4]);

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
            return NextOccurrence(_crondate);
        }

        public DateTime NextOccurrence(DateTime date)
        {
            _crondate = GetNextOccurrence(Truncate(date.AddMinutes(1)));

            var validator = GetNextOccurrence(_crondate);

            while (validator != _crondate)
            {
                _crondate = validator;
                validator = GetNextOccurrence(_crondate);
            }

            return _crondate;
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
            return PreviousOccurrence(_crondate);
        }

        public DateTime PreviousOccurrence(DateTime date)
        {
            _crondate = GetPreviousOccurrence(Truncate(date.AddMinutes(-1)));

            var validator = GetPreviousOccurrence(_crondate);

            while (validator != _crondate)
            {
                _crondate = validator;
                validator = GetPreviousOccurrence(_crondate);
            }

            return _crondate;
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