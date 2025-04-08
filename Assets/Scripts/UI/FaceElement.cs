using System;
using System.Collections.Generic;
using System.Linq;
using Renderers;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    
    public class FaceElement : ListIndexedElement<IndexedPolygon>
    {
        protected override UnityEvent<UiEvents.IndexListEventData> Event() => UiEvents.polygonEvent;
        
        protected override IEnumerable<int> Indexes(IndexedPolygon data) => data.Data.indexes;
    }
}