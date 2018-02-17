using System;
using System.Text.RegularExpressions;

namespace Shuttle.Core.Cron
{
    public class CronDayOfWeek : CronField
    {
        private static readonly Regex MonthExpression = new Regex("sun|mon|tue|wed|thu|fri|sat|^l$",
            RegexOptions.IgnoreCase);

        private readonly Regex _weekdayOccurrenceExpression = new Regex(@"^(?<day>\d)\#(?<occurrence>\d)$",
            RegexOptions.IgnoreCase);

        public CronDayOfWeek(string value)
            : base(MonthExpression.Replace(value,
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
                }))
        {
            switch (value.ToLower())
            {
                case "?":
                {
                    ExpressionType = ExpressionType.Skipped;

                    // will be determined by day-of-week field
                    return;
                }
            }

            var match = _weekdayOccurrenceExpression.Match(value);

            if (match.Success)
            {
                ExpressionType = ExpressionType.WeekDayOccurrence;

                AddSpecification(new WeekDayOccurenceSpecification(Convert.ToInt32(match.Groups["day"].Value),
                    Convert.ToInt32(match.Groups["occurrence"].Value)));

                return;
            }

            DefaultParsing(1, 7);
        }

        public override DateTime SnapForward(DateTime date)
        {
            return Snap(date, 1);
        }

        public override DateTime SnapBackward(DateTime date)
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
                case ExpressionType.WeekDayOccurrence:
                {
                    while (!IsSatisfiedBy(date))
                    {
                        date = date.AddDays(delta);
                    }

                    break;
                }
                default:
                {
                    while (!IsSatisfiedBy((int) date.DayOfWeek + 1))
                    {
                        date = date.AddDays(delta);
                    }

                    break;
                }
            }

            return date;
        }
    }
}