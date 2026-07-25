using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static List<GameObject> SortByDistance(this List<GameObject> objects, GameObject comparable)
    {
        Vector3 pos = comparable.transform.position;
        objects.Sort((x, y) => (pos - x.transform.position).sqrMagnitude.CompareTo((pos - y.transform.position).sqrMagnitude));
        return objects;
    }
}

