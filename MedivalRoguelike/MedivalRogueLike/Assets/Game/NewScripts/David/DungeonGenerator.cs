using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
	public int m_X = 0;
	public int m_Y = 0;
	public int m_W = 0;
	public int m_H = 0;
	public Room m_ConnectedTo = null;
	//public int branch = 0;
	public string m_Relative_positioning = "x";
	public bool m_Dead_end = false;
	public int m_Room_id = 0;
}

public class MapTile
{
	public int m_Type = 0; //Default = 0 , Room Floor = 1, Wall = 2, Corridor Floor 3, Room Corners = 4, 5, 6 , 7
	public int m_Orientation = 0;
	public Room m_Room = null;
}

public class DungeonGenerator : MonoBehaviour
{
	[Tooltip("Spawns the selected prefab at the beginning of the dungeon(usually used for stuff like player spawning)")]
	public GameObject m_StartPrefab;
	[Tooltip("Spawns the selected prefab at the end of the dungeon(usually used for stuff like proceeding to next level)")]
	public GameObject m_ExitPrefab;
	[Tooltip("The floor used in non custom rooms.")]
	public GameObject m_FloorPrefab;
	[Tooltip("The floor used in corridor's")]
	public GameObject corridorFloorPrefab;
	public GameObject m_WallPrefab;
	public GameObject m_DoorPrefab;

	public GameObject m_CornerPrefab;
	public bool m_CornerRotation = false;

	public int m_MaximumRoomCount = 10;

	[Tooltip("Maximum gap between rooms. Also affects corridor lengths ")]
	public int m_RoomMargin = 3;

	[Tooltip("If Checked: makes dungeon reset on every time level loads.")]
	public bool m_Generate_on_load = true;
	public int m_MinRoomSize = 5;
	public int m_MaxRoomSize = 10;
	public float m_TileScaling = 1f;
	public List<SpawnOption> m_SpawnOptions = new List<SpawnOption>();
	public List<CustomRoom> m_CustomRooms = new List<CustomRoom>();
	public bool m_MakeIt3d = false;

	// private class within dungeongenerator so it can't be called from the outside
	private class Dungeon
	{
		public int m_MapSize;
		public int m_MapSize_X;
		public int m_MapSize_Y;


	}
}
