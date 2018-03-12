using System;
using System.Collections.Generic;

namespace Collate.Implementation
{
    public class FilterRequest : IFilterRequest
    {
        public FilterLogic Logic { get; set; }

        public IEnumerable<IFilter> Filters { get; set; }

        public FilterRequest()
        {
            Logic = FilterLogic.And;
            Filters = Array.Empty<IFilter>();
        }
    }
}
