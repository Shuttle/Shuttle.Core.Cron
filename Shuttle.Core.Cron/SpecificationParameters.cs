namespace Shuttle.Core.Cron
{
    public class SpecificationParameters
    {
        public FieldName FieldName { get; }
        public string Value { get; }

        public SpecificationParameters(FieldName fieldName, string value)
        {
            FieldName = fieldName;
            Value = value;
        }
    }
}