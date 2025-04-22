using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Structures_Geométriques.Extensions;

[Serializable]
public struct Polygone
{
    public List<Point> sommets; // Liste des sommets du polygone
    private Vector3 _gravityCenter;

    public Polygone(IEnumerable<Point> sommetsI)
    {
        this.sommets = new List<Point>(sommetsI);
        Vector3 center = Vector3.zero;
        for (int i = 0; i < sommets.Count; i++)
        {
            center += sommets[i].ToVector3();
        }

        _gravityCenter = center / sommets.Count;
    }

    public Vector3 GravityCenter
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