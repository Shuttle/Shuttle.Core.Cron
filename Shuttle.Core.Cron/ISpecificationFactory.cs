using Shuttle.Core.Specification;

namespace Shuttle.Core.Cron;

public interface ISpecificationFactory
{
    ISpecification<CronField.Candidate>? Create(SpecificationParameters parameters);
}