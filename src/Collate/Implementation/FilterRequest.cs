using System.Collections.Generic;

namespace Collate.Implementation
{
    public class FilterRequest : IFilterRequest
    {
        public FilterLogic Logic { get; set; }

        public IEnumerable<IFilter> Filters { get; set; }
    }
}
