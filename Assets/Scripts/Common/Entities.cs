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

public enum EEnvironmentBlock : int
{
    None,
    Wall,
    Ground,
    StartPoint,
    ExitPoint,
    WayPoint,
}
