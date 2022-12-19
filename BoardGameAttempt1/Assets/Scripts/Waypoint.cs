using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public WayPointType type;

    public WayPointType GetWayPointType()
    {
        if(type == WayPointType.Blue)
        {
            return WayPointType.Blue;
        }
        else if (type == WayPointType.Red)
        {
            return WayPointType.Red;
        }
        else if (type == WayPointType.Yellow)
        {
            return WayPointType.Yellow;
        }
        else if (type == WayPointType.Green)
        {
            return WayPointType.Green;
        }
        return WayPointType.Blue;
    }
}

public enum WayPointType
{
    Blue,
    Red,
    Yellow,
    Green
}