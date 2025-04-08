using System.Collections.Generic;
using UnityEngine.Events;

namespace UI
{
    public class PolyedreElement : ListIndexedElement<IndexedPolyedre>
    {
        protected override UnityEvent<UiEvents.IndexListEventData> Event() => UiEvents.polyedreEvent;
        
        protected override IEnumerable<int> Indexes(IndexedPolyedre data) => data.Data.indexes;
    }
}