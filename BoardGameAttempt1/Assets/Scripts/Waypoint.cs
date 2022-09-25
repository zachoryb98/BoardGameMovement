using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public WayPointType type;

    public string GetWayPointType()
    {
        if(type == WayPointType.Blue)
        {
            return "Blue";
        }
        else if (type == WayPointType.Red)
        {
            return "Red";
        }
        else if (type == WayPointType.Yellow)
        {
            return "Yellow";
        }
        else if (type == WayPointType.Green)
        {
            return "Green";
        }
        return "Not a valid color";
    }
}

public enum WayPointType
{
    Blue,
    Red,
    Yellow,
    Green
}