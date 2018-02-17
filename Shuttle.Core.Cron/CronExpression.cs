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
        public DateTime Crondate;

        public CronExpression(string expression)
            : this(expression, DateTime.Now)
        {
        }

        public CronExpression(string expression, DateTime date)
        {
            Guard.AgainstNullOrEmptyString(expression, nameof(expression));

            Expression = expression;

            Crondate = RemoveSeconds(date);

            ParseExpression(expression);
        }

        public string Expression { get; }

        private static DateTime RemoveSeconds(DateTime date)
        {
            return date.AddSeconds(date.Second * -1);
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
            return NextOccurrence(Crondate);
        }

        public DateTime NextOccurrence(DateTime date)
        {
            Crondate = SnapNextOccurrence(RemoveSeconds(date.AddMinutes(1)));

            var validator = SnapNextOccurrence(Crondate);

            while (validator != Crondate)
            {
                Crondate = validator;
                validator = SnapNextOccurrence(Crondate);
            }

            return Crondate;
        }

        private DateTime SnapNextOccurrence(DateTime date)
        {
            var result = date;

            result = _cronMinute.SnapForward(result);
            result = _cronHour.SnapForward(result);
            result = _cronDayOfMonth.SnapForward(result);
            result = _cronMonth.SnapForward(result);
            result = _cronDayOfWeek.SnapForward(result);

            return result;
        }

        public DateTime PreviousOccurrence()
        {
            return PreviousOccurrence(Crondate);
        }

        public DateTime PreviousOccurrence(DateTime date)
        {
            Crondate = SnapPreviousOccurrence(RemoveSeconds(date.AddMinutes(-1)));

            var validator = SnapPreviousOccurrence(Crondate);

            while (validator != Crondate)
            {
                Crondate = validator;
                validator = SnapPreviousOccurrence(Crondate);
            }

            return Crondate;
        }

        private DateTime SnapPreviousOccurrence(DateTime date)
        {
            var result = date;

            result = _cronMinute.SnapBackward(result);
            result = _cronHour.SnapBackward(result);
            result = _cronDayOfMonth.SnapBackward(result);
            result = _cronMonth.SnapBackward(result);
            result = _cronDayOfWeek.SnapBackward(result);

            return result;
        }
    }
}