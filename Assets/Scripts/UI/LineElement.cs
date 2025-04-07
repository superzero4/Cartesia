using UnityEngine;

namespace UI
{
    public class LineElement : Element<IndexedSegment>
    {
        [SerializeField] private IndexText _pointA;
        [SerializeField] private IndexText _pointB;

        public override void RefreshView()
        {
            _pointA.SetIndex(Data.Data.x);
            _pointB.SetIndex(Data.Data.y);
        }

    }
}