using System;
using System.Text.RegularExpressions;

namespace Shuttle.Core.Cron;

public class CronDayOfMonth : CronField
{
    private readonly Regex _weekdayExpression = new(@"^(?<day>\d+)W$", RegexOptions.IgnoreCase);

    public CronDayOfMonth(string expression, ISpecificationFactory? specificationFactory = null) : base(expression, specificationFactory)
    {
        switch (expression.ToLower())
        {
            case "?":
            {
                ExpressionType = ExpressionType.Skipped;

                // will be determined by day-of-week field
                return;
            }
            case "l":
            {
                ExpressionType = ExpressionType.LastDayOfMonth;

                AddSpecification(new LastDayOfMonthSpecification());

                return;
            }
            case "lw":
            {
                ExpressionType = ExpressionType.LastWeekDayOfMonth;

                AddSpecification(new LastWeekDayOfMonthSpecification());

                return;
            }
        }

        var match = _weekdayExpression.Match(expression);

        if (match.Success)
        {
            ExpressionType = ExpressionType.NearestWeekDay;

            AddSpecification(new RangeSpecification(Convert.ToInt32(match.Groups["day"].Value)));

            return;
        }

        DefaultParsing(FieldName.DayOfMonth, 1, 31);
    }

    public override DateTime GetNext(DateTime date)
    {
        return Snap(date, 1);
    }

    public override DateTime GetPrevious(DateTime date)
    {
        return Snap(date, -1);
    }

    private DateTime Snap(DateTime date, int delta)
    {
        switch (ExpressionType)
        {
            case ExpressionType.Skipped:
            {
                return date;
            }
            case ExpressionType.NearestWeekDay:
            {
                while (!IsSatisfiedBy(new(FieldName.DayOfMonth, Expression, date)))
                {
                    date = date.AddDays(delta);
                }

                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Saturday:
                    {
                        if (date.Day == 1)
                        {
                            // 1st of month and since we don't cross months we'll move to the next Monday
                            date = date.AddDays(2);
                        }
                        else
                        {
                            // move to the previous Friday
                            date = date.AddDays(-1);
                        }

                        break;
                    }
                    case DayOfWeek.Sunday:
                    {
                        if (date.Day == DateTime.DaysInMonth(date.Year, date.Month))
                        {
                            // last day of month and since we don't cross months we'll move to the previous Friday
                            date = date.AddDays(-2);
                        }
                        else
                        {
                            // move to the next Monday
                            date = date.AddDays(1);
                        }

                        break;
                    }
                }

                break;
            }
            default:
            {
                while (!IsSatisfiedBy(new(FieldName.DayOfMonth, Expression, date)))
                {
                    date = date.AddDays(delta);
                }

                break;
            }
        }

        return date;
    }
}