using System;

namespace Shuttle.Core.Cron;

public class CronMinute : CronField
{
    public CronMinute(string expression, ISpecificationFactory? specificationFactory = null) : base(expression, specificationFactory)
    {
        DefaultParsing(FieldName.Minute, 0, 59);
    }

    public override DateTime GetNext(DateTime date)
    {
        while (!IsSatisfiedBy(new(FieldName.Minute, Expression, date)))
        {
            date = date.AddMinutes(1);
        }

        return date;
    }

    public override DateTime GetPrevious(DateTime date)
    {
        while (!IsSatisfiedBy(new(FieldName.Minute, Expression, date)))
        {
            date = date.AddMinutes(-1);
        }

        return date;
    }
}