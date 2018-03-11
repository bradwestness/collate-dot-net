namespace Collate.Implementation
{
    public class Filter : IFilter
    {
        public FilterOperator Operator { get; set; }

        public string Field { get; set; }

        public string Value { get; set; }
    }
}
