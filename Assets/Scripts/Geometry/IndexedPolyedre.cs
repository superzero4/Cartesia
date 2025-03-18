using System;
using System.Collections.Generic;
using System.Linq;
using Structures_Geométriques;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class IndexedPolyedre : Dirty<Indexes, Polyedre>
{
    private List<IndexedPolygon> _faces;

    public void SetPolygons(List<IndexedPolygon> faces)
    {
        _faces = faces;
        this.MarkDirty();
    }

    public override Polyedre Compute(Indexes tSource)
    {
        return new Polyedre(tSource.indexes.Select(index => _faces[index].Value) );
    }
}