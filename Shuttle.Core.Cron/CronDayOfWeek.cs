using System;
using System.Text.RegularExpressions;

namespace Shuttle.Core.Cron;

public class CronDayOfWeek : CronField
{
    private static readonly Regex NameExpression = new("sun|mon|tue|wed|thu|fri|sat|^l$",
        RegexOptions.IgnoreCase);

    private static readonly Regex OccurrenceExpression = new(@"^(?<day>\d)\#(?<occurrence>\d)$",
        RegexOptions.IgnoreCase);

    public CronDayOfWeek(string value, ISpecificationFactory? specificationFactory = null)
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
            default:
            {
                while (!IsSatisfiedBy(new(FieldName.DayOfWeek, Expression, dateTimeOffset)))
                {
                    dateTimeOffset = dateTimeOffset.AddDays(delta);
                }

                break;
            }
        }

        return dateTimeOffset;
    }
}