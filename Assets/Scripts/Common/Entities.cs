using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Edirection : int
{
    Up, 
    Right,
    Down, 
    Left,
}


public enum EEnvironmentBlock
{
    None        = 0,
    Wall        = 1,
    Ground      = 2,
    StartPoint  = 4,
    ExitPoint   = 8,
    Waypoint1   = 16,
    Waypoint2   = 32,
    Waypoint3   = 64,
    HasDoor     = 128,
    HasVariant1 = 256,
    HasVariant2 = 512,
    HasVariant3 = 1024,
}
