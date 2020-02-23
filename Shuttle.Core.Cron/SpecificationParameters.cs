namespace Shuttle.Core.Cron
{
    public class SpecificationParameters
    {
        public FieldName FieldName { get; }
        public string Expression { get; }

        public SpecificationParameters(FieldName fieldName, string expression)
        {
            FieldName = fieldName;
            Expression = expression;
        }
    }
}