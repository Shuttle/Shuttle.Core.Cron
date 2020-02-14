using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Specification;

namespace Shuttle.Core.Cron
{
    public class DefaultSpecificationFactory : ISpecificationFactory
    {
        private readonly Func<SpecificationParameters, ISpecification<CronField.Candidate>> _creator;

        public DefaultSpecificationFactory(Func<SpecificationParameters, ISpecification<CronField.Candidate>> creator)
        {
            _creator = creator;
        }

        public DefaultSpecificationFactory()
        {
        }

        public ISpecification<CronField.Candidate> Create(SpecificationParameters parameters)
        {
            Guard.AgainstNull(parameters, nameof(parameters));

            if (_creator == null)
            {
                throw new CronException(string.Format(Resources.InvalidDefaultSpecificationFactoryConfiguration,
                    parameters.Value, parameters.FieldName));
            }

            return _creator.Invoke(parameters);
        }
    }
}