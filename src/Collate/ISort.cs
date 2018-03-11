namespace Collate
{
    public interface ISort
    {
        SortDirection Direction { get; }
        string Field { get; }
    }
}
