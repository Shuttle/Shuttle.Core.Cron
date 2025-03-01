using System;

namespace Shuttle.Core.Cron;

public class CronMinute : CronField
{
    public CronMinute(string expression, ISpecificationFactory? specificationFactory = null) : base(expression, specificationFactory)
    {
        DefaultParsing(FieldName.Minute, 0, 59);
    }

    public override DateTimeOffset GetNext(DateTimeOffset dateTimeOffset)
    {
        while (!IsSatisfiedBy(new(FieldName.Minute, Expression, dateTimeOffset)))
        {
            dateTimeOffset = dateTimeOffset.AddMinutes(1);
        }

        return dateTimeOffset;
    }

    public override DateTimeOffset GetPrevious(DateTimeOffset dateTimeOffset)
    {
        while (!IsSatisfiedBy(new(FieldName.Minute, Expression, dateTimeOffset)))
        {
            dateTimeOffset = dateTimeOffset.AddMinutes(-1);
        }

        return dateTimeOffset;
    }
}