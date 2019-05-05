using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRooms : MonoBehaviour
{
    [Tooltip("Rooms with doors on each side")]
    public List<GameObject> LTRB;
    [Tooltip("Rooms with doors on the left")]
    public List<GameObject> L;
    [Tooltip("Rooms with doors on the top")]
    public List<GameObject> T;
    [Tooltip("Rooms with doors on the right")]
    public List<GameObject> R;
    [Tooltip("Rooms with doors on the bottom")]
    public List<GameObject> B;
    [Tooltip("Rooms with doors on the left and top")]
    public List<GameObject> LT;
    [Tooltip("Rooms with doors on top and right")]
    public List<GameObject> TR;
    [Tooltip("Rooms with doors on the right and bottom")]
    public List<GameObject> RB;
    [Tooltip("Rooms with doors on bottom and left")]
    public List<GameObject> BL;
    [Tooltip("Rooms with doors on the left and right")]
    public List<GameObject> LR;
    [Tooltip("Rooms with doors on top and bottom")]
    public List<GameObject> TB;
}
