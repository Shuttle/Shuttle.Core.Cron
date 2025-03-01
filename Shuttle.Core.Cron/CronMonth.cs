using System;
using System.Text.RegularExpressions;

namespace Shuttle.Core.Cron;

public class CronMonth : CronField
{
    private static readonly Regex MonthExpression = new("jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec",
        RegexOptions.IgnoreCase);

    public CronMonth(string value, ISpecificationFactory? specificationFactory = null)
        : base(MonthExpression.Replace(value,
            match =>
            {
                switch (match.Value.ToLower())
                {
                    case "jan":
                    {
                        return "1";
                    }
                    case "feb":
                    {
                        return "2";
                    }
                    case "mar":
                    {
                        return "3";
                    }
                    case "apr":
                    {
                        return "4";
                    }
                    case "may":
                    {
                        return "5";
                    }
                    case "jun":
                    {
                        return "6";
                    }
                    case "jul":
                    {
                        return "7";
                    }
                    case "aug":
                    {
                        return "8";
                    }
                    case "sep":
                    {
                        return "9";
                    }
                    case "oct":
                    {
                        return "10";
                    }
                    case "nov":
                    {
                        return "11";
                    }
                    default:
                    {
                        return "12";
                    }
                }
            }), specificationFactory)
    {
        DefaultParsing(FieldName.Month, 1, 12);
    }

    public override DateTimeOffset GetNext(DateTimeOffset dateTimeOffset)
    {
        while (!IsSatisfiedBy(new(FieldName.Month, Expression, dateTimeOffset)))
        {
            dateTimeOffset = dateTimeOffset.AddMonths(1);
        }

        return dateTimeOffset;
    }

    public override DateTimeOffset GetPrevious(DateTimeOffset dateTimeOffset)
    {
        while (!IsSatisfiedBy(new(FieldName.Month, Expression, dateTimeOffset)))
        {
            dateTimeOffset = dateTimeOffset.AddMonths(-1);
        }

        return dateTimeOffset;
    }
}