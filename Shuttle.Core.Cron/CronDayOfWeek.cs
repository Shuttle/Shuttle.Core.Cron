using System;
using System.Text.RegularExpressions;

namespace Shuttle.Core.Cron;

public class CronDayOfWeek : CronField
{
    private static readonly Regex NameExpression = new("sun|mon|tue|wed|thu|fri|sat|^l$",
        RegexOptions.IgnoreCase);

    private static readonly Regex OccurrenceExpression = new(@"^(?<day>\d)\#(?<occurrence>\d)$",
        RegexOptions.IgnoreCase);

    public CronDayOfWeek(string value, ISpecificationFactory specificationFactory = null)
        : base(NameExpression.Replace(value,
            match =>
            {
                switch (match.Value.ToLower())
                {
                    case "sun":
                    {
                        return "1";
                    }
                    case "mon":
                    {
                        return "2";
                    }
                    case "tue":
                    {
                        return "3";
                    }
                    case "wed":
                    {
                        return "4";
                    }
                    case "thu":
                    {
                        return "5";
                    }
                    case "fri":
                    {
                        return "6";
                    }
                    default:
                    {
                        return "7";
                    }
                }
            }), specificationFactory)
    {
        if (value.Equals("?"))
        {
            ExpressionType = ExpressionType.Skipped;

            // will be determined by day-of-week field
            return;
        }

        var match = OccurrenceExpression.Match(value);

        if (match.Success)
        {
            ExpressionType = ExpressionType.WeekDayOccurrence;

            AddSpecification(new WeekDayOccurrenceSpecification(Convert.ToInt32(match.Groups["day"].Value),
                Convert.ToInt32(match.Groups["occurrence"].Value)));

            return;
        }

        DefaultParsing(FieldName.DayOfWeek, 1, 7);
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
            default:
            {
                while (!IsSatisfiedBy(new(FieldName.DayOfWeek, Expression, date)))
                {
                    date = date.AddDays(delta);
                }

                break;
            }
        }

        return date;
    }
}