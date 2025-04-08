using Renderers;
using System;
using System.Collections.Generic;
using System.Drawing;
using Control;
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

    private int CyclicIncrement(int x, bool increment, int max)
    {
        return (x + (increment ? 1 : -1) + max) % max;
    }

    public void UpdatePointInPolygon(UiEvents.IndexListEventData arg0)
    {
        var polygon = _relativeGeometry._polygons[arg0.objectIndex];
        var data = polygon.Data;
        data.indexes[arg0.indexInObject] =
            CyclicIncrement(polygon.Data.indexes[arg0.indexInObject], arg0.up, _relativeGeometry._points.Count);
        polygon.Data = data;
    }

    public void UpdatePointInPolyedre(UiEvents.IndexListEventData arg0)
    {
        var polyedre = _relativeGeometry._polyedres[arg0.objectIndex];
        var data = polyedre.Data;
        data.indexes[arg0.indexInObject] =
            CyclicIncrement(polyedre.Data.indexes[arg0.indexInObject], arg0.up, _relativeGeometry._points.Count);
        polyedre.Data = data;
    }
}