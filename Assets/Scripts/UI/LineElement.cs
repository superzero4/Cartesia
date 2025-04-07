using Control;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class LineElement : Element<IndexedSegment>
    {
        [SerializeField] private IncrementalIndex _pointA;
        [SerializeField] private IncrementalIndex _pointB;

        public override void SetData(IndexedSegment data, int index)
        {
            base.SetData(data, index);
            _pointA.SetIndex(data.Data.x);
            _pointB.SetIndex(data.Data.y);
        }

        public void Awake()
        {
            _pointA.SetEvent(UiEvents.lineEvent, new UiEvents.LineEventData()
            {
                objectIndex = Index,
                isFirst = true
            });
            _pointB.SetEvent(UiEvents.lineEvent, new UiEvents.LineEventData()
            {
                objectIndex = Index,
                isFirst = false
            });
        }
    }
}