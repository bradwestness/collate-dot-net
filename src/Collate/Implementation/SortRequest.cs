using System.Collections.Generic;

namespace Collate.Implementation
{
    public class SortRequest : ISortRequest
    {
        public IEnumerable<ISort> Sorts { get; set; }
    }
}
