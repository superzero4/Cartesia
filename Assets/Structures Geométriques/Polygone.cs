using System;
using System.Collections.Generic;

[Serializable]
public struct Polygone
{
    public List<Point> sommets; // Liste des sommets du polygone
    private Point _gravityCenter;

    public Polygone(IEnumerable<Point> sommetsI)
    {
        this.sommets = new List<Point>(sommetsI);
        _gravityCenter = new Point(0,0,0);
        for (int i = 0; i < sommets.Count; i++)
        {
            _gravityCenter = new Point(_gravityCenter.x + sommets[i].x, _gravityCenter.y + sommets[i].y, _gravityCenter.z + sommets[i].z);
        }

        _gravityCenter = new Point(_gravityCenter.x / sommets.Count, _gravityCenter.y / sommets.Count, _gravityCenter.z / sommets.Count);
    }

    public Point GravityCenter
    {
        get { return _gravityCenter; }
    }

    // Calculer le périmètre du polygone
    public float Perimeter()
    {
        if (sommets.Count < 3) return 0;

        float perimeter = 0;
        for (int i = 0; i < sommets.Count; i++)
        {
            Point current = sommets[i];
            Point next = sommets[(i + 1) % sommets.Count];

            perimeter += current.Distance(next);
        }

        return perimeter;
    }

    // Afficher les points du polygone
    public override string ToString()
    {
        string result = "Polygone : ";
        foreach (var point in sommets)
        {
            result += point.ToString() + " -> ";
        }

        return result + sommets[0].ToString();
    }
}