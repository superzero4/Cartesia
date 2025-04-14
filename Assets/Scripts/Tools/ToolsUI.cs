using Renderers;
using System;
using System.Collections.Generic;
using System.Drawing;
using Control;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class ToolsUI
{
    private RelativeGeometry _relativeGeometry;

    public ToolsUI(RelativeGeometry relative_geometry)
    {
        _relativeGeometry = relative_geometry;
    }

    // S�lectionner le point suivant dans la liste
    public void UpdatePointInLine(UiEvents.LineEventData arg0)
    {
        var line = _relativeGeometry._lines[arg0.objectIndex];
        var v2 = line.Data;
        var max = _relativeGeometry._points.Count;
        if (arg0.isFirst)
            v2.x = CyclicIncrement(v2.x, arg0.up, max);
        else
            v2.y = CyclicIncrement(v2.y, arg0.up, max);
        line.Data = v2;
    }

    private int CyclicIncrement(int x, bool increment, int? max = null)
    {
        if (!max.HasValue)
            max = _relativeGeometry._points.Count;
        return (x + (increment ? 1 : -1) + max.Value) % max.Value;
    }

    public void UpdatePointInPolygon(UiEvents.IndexListEventData arg0)
    {
        var polygon = _relativeGeometry._polygons[arg0.objectIndex];
        HandleIndexList(arg0, polygon.Data.indexes);
    }

    public void UpdatePointInPolyedre(UiEvents.IndexListEventData arg0)
    {
        var polyedre = _relativeGeometry._polyedres[arg0.objectIndex];
        HandleIndexList(arg0, polyedre.Data.indexes);
    }

    private void HandleIndexList(UiEvents.IndexListEventData arg0, List<int> indexes)
    {
        if (arg0.indexInObject < indexes.Count)
            indexes[arg0.indexInObject] =
                CyclicIncrement(indexes[arg0.indexInObject], arg0.up);
        else if (arg0.indexInObject == indexes.Count)//We create a new point, equal last + 1 by default
            indexes.Add(CyclicIncrement(indexes[arg0.indexInObject - 1], true));
        else
            Assert.IsTrue(false, "Index out of range");
    }
}