namespace Shuttle.Core.Cron;

public class SpecificationParameters
{
    public SpecificationParameters(FieldName fieldName, string expression)
    {
        FieldName = fieldName;
        Expression = expression;
    }

    public string Expression { get; }
    public FieldName FieldName { get; }
}