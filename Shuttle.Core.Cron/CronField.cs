using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Shuttle.Core.Contract;
using Shuttle.Core.Specification;

namespace Shuttle.Core.Cron;

public enum ExpressionType
{
    Default,
    All,
    LastDayOfMonth,
    LastWeekDayOfMonth,
    LastWeekDay,
    WeekDayOccurrence,
    NearestWeekDay,
    LastDayOfWeek,
    Skipped
}

public abstract class CronField : ISpecification<CronField.Candidate>
{
    private readonly ISpecificationFactory? _specificationFactory;

    private readonly List<ISpecification<Candidate>> _specifications = new();

    protected readonly Regex RangeExpression = new(@"^(?<start>\d+)-(?<end>\d+)/(?<step>\d+)$|^(?<start>\d+)-(?<end>\d+)$|^(?<start>\d+)$|^(?<start>\d+)/(?<step>\d+)$|^(?<start>\*)/(?<step>\d+)$|^(?<start>\*)$", RegexOptions.IgnoreCase);

    protected CronField(string expression, ISpecificationFactory? specificationFactory = null)
    {
        Expression = Guard.AgainstNullOrEmptyString(expression);
        _specificationFactory = specificationFactory;
    }

    public string Expression { get; }
    public ExpressionType ExpressionType { get; protected set; }

    public virtual bool IsSatisfiedBy(Candidate item)
    {
        return !_specifications.Any() || _specifications.Any(specification => specification.IsSatisfiedBy(item));
    }

    protected void AddSpecification(ISpecification<Candidate> specification)
    {
        Guard.AgainstNull(specification);

        _specifications.Add(specification);
    }

    protected void DefaultParsing(FieldName fieldName, int minimum, int maximum)
    {
        ExpressionType = ExpressionType.Default;

        foreach (var s in SplitValue())
        {
            var match = RangeExpression.Match(s);

            if (!match.Success)
            {
                Guard.Against<CronException>(_specificationFactory == null, string.Format(Resources.CronInvalidExpression, s));

                var specification = _specificationFactory!.Create(new(fieldName, s));

                Guard.Against<CronException>(specification == null, string.Format(Resources.NullSpecificationFromFactory, fieldName, s));
                
                AddSpecification(specification!);

                ExpressionType = ExpressionType.All;

                return;
            }

            var startValue = match.Groups["start"].Value;
            var endValue = match.Groups["end"].Value;
            var stepValue = match.Groups["step"].Value;

            var step = string.IsNullOrEmpty(stepValue)
                ? 1
                : Convert.ToInt32(stepValue);

            if (startValue == "*")
            {
                AddSpecification(new RangeSpecification(0, maximum, step));

                ExpressionType = ExpressionType.All;

                return;
            }

            var start = Convert.ToInt32(startValue);
            var end = string.IsNullOrEmpty(endValue)
                ? string.IsNullOrEmpty(stepValue)
                    ? start
                    : maximum
                : Convert.ToInt32(endValue);

            Guard.Against<CronException>(start < minimum, string.Format(Resources.CronStartValueTooSmall, start, minimum));
            Guard.Against<CronException>(end < minimum, string.Format(Resources.CronEndValueTooSmall, end, minimum));
            Guard.Against<CronException>(start > maximum, string.Format(Resources.CronStartValueTooLarge, start, maximum));
            Guard.Against<CronException>(end > maximum, string.Format(Resources.CronEndValueTooLarge, end, maximum));

            AddSpecification(new RangeSpecification(start, end, step));
        }
    }

    public abstract DateTime GetNext(DateTime date);
    public abstract DateTime GetPrevious(DateTime date);

    protected string[] SplitValue()
    {
        return Expression.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
    }

    public class Candidate
    {
        public Candidate(FieldName fieldName, string expression, DateTime date)
        {
            FieldName = Guard.AgainstUndefinedEnum<FieldName>(fieldName, nameof(fieldName));
            Expression = Guard.AgainstNullOrEmptyString(expression, nameof(expression));
            Date = date;
        }

        public DateTime Date { get; }
        public string Expression { get; }
        public FieldName FieldName { get; }
    }
}