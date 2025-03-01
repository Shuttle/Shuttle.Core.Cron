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

    public override DateTimeOffset GetNext(DateTimeOffset dateTimeOffset)
    {
        return Snap(dateTimeOffset, 1);
    }

    public override DateTimeOffset GetPrevious(DateTimeOffset dateTimeOffset)
    {
        return Snap(dateTimeOffset, -1);
    }

    private DateTimeOffset Snap(DateTimeOffset dateTimeOffset, int delta)
    {
        switch (ExpressionType)
        {
            case ExpressionType.Skipped:
            {
                return dateTimeOffset;
            }
            case ExpressionType.NearestWeekDay:
            {
                while (!IsSatisfiedBy(new(FieldName.DayOfMonth, Expression, dateTimeOffset)))
                {
                    dateTimeOffset = dateTimeOffset.AddDays(delta);
                }

                switch (dateTimeOffset.DayOfWeek)
                {
                    case DayOfWeek.Saturday:
                    {
                        if (dateTimeOffset.Day == 1)
                        {
                            // 1st of month and since we don't cross months we'll move to the next Monday
                            dateTimeOffset = dateTimeOffset.AddDays(2);
                        }
                        else
                        {
                            // move to the previous Friday
                            dateTimeOffset = dateTimeOffset.AddDays(-1);
                        }

                        break;
                    }
                    case DayOfWeek.Sunday:
                    {
                        if (dateTimeOffset.Day == DateTime.DaysInMonth(dateTimeOffset.Year, dateTimeOffset.Month))
                        {
                            // last day of month and since we don't cross months we'll move to the previous Friday
                            dateTimeOffset = dateTimeOffset.AddDays(-2);
                        }
                        else
                        {
                            // move to the next Monday
                            dateTimeOffset = dateTimeOffset.AddDays(1);
                        }

                        break;
                    }
                }

                break;
            }
            default:
            {
                while (!IsSatisfiedBy(new(FieldName.DayOfMonth, Expression, dateTimeOffset)))
                {
                    dateTimeOffset = dateTimeOffset.AddDays(delta);
                }

                break;
            }
        }

        return dateTimeOffset;
    }
}