using System;

namespace Shuttle.Core.Cron;

public class CronHour : CronField
{
    public CronHour(string expression, ISpecificationFactory? specificationFactory = null) : base(expression, specificationFactory)
    {
        DefaultParsing(FieldName.Hour, 0, 23);
    }

    public override DateTimeOffset GetNext(DateTimeOffset dateTimeOffset)
    {
        while (!IsSatisfiedBy(new(FieldName.Hour, Expression, dateTimeOffset)))
        {
            dateTimeOffset = dateTimeOffset.AddHours(1);
        }

        return dateTimeOffset;
    }

    public override DateTimeOffset GetPrevious(DateTimeOffset dateTimeOffset)
    {
        while (!IsSatisfiedBy(new(FieldName.Hour, Expression, dateTimeOffset)))
        {
            dateTimeOffset = dateTimeOffset.AddHours(-1);
        }

        return dateTimeOffset;
    }
}