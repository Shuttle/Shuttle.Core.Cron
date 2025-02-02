using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Specification;

namespace Shuttle.Core.Cron;

public class SpecificationFactory : ISpecificationFactory
{
    private readonly Func<SpecificationParameters, ISpecification<CronField.Candidate>?>? _factory;

    public SpecificationFactory(Func<SpecificationParameters, ISpecification<CronField.Candidate>?> factory)
    {
        _factory = Guard.AgainstNull(factory);
    }

    public SpecificationFactory()
    {
    }

    public ISpecification<CronField.Candidate>? Create(SpecificationParameters parameters)
    {
        Guard.AgainstNull(parameters);

        if (_factory == null)
        {
            throw new CronException(string.Format(Resources.InvalidDefaultSpecificationFactoryConfiguration, parameters.Expression, parameters.FieldName));
        }

        return _factory.Invoke(parameters);
    }
}