using System.Collections.Generic;

namespace Collate
{
    public interface ISortRequest
    {
        IEnumerable<ISort> Sorts { get; }
    }
}
