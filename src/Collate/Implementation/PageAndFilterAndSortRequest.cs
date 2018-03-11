using System.Collections.Generic;

namespace Collate.Implementation
{
    public class PageAndFilterAndSortRequest : IPageRequest, IFilterRequest, ISortRequest
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public FilterLogic Logic { get; set; }

        public IEnumerable<IFilter> Filters { get; set; }

        public IEnumerable<ISort> Sorts { get; set; }
    }
}
