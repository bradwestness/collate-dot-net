namespace Collate
{
    public interface IFilter
    {
        FilterOperator Operator { get; }
        string Field { get; }
        string Value { get; }
    }
}
