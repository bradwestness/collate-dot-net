using System.Collections.Generic;

namespace Collate
{
    public interface IFilterRequest
    {
        FilterLogic Logic { get; }
        IEnumerable<IFilter> Filters { get; }
    }
}
