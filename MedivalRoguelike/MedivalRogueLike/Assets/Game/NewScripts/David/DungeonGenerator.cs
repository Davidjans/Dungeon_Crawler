using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// specifications of each individual room.
public class Room
{
	public int m_X = 0;
	public int m_Y = 0;
	public int m_W = 0;
	public int m_H = 0;
	public Room m_ConnectedTo = null;
	//public int branch = 0;
	public string m_Relative_positioning = "x";
	public bool m_DeadEnd = false;
	public int m_RoomId = 0;
}

// 
public class SpawnList
{
	public int m_X;
	public int m_Y;
	public bool m_ByWall;
	public string m_WallLocation;
	public bool m_InTheMiddle;
	public bool m_ByCorridor;

	public int m_AsDoor = 0;
	public Room m_Room = null;
	public bool m_SpawnedObject;
}

// extra spawn options.
[System.Serializable]
public class SpawnOption
{
	public int m_MinSpawnCount;
	public int m_MaxSpawnCount;
	public bool m_SpawnByWall;
	public bool m_SpawnInMiddle;
	public bool m_SpawnRotated;

	public float m_HeightFix = 0;

    public GameObject gameObject;
	// not 100 % sure the functionality of this yet
	public GameObject m_ObjectToSpawn;
	[Tooltip("Use 0 for random room, make sure spawn room isnt bigger than your room count")]
	public int m_SpawnRoom = 0;
}

// select the prefabs you want to use in your custom room with this.
[System.Serializable]
public class CustomRoom
{
	[Tooltip("make sure room id isnt bigger than your room count")]
	public int roomId = 1;
	public GameObject floorPrefab;
	public GameObject wallPrefab;
	public GameObject doorPrefab;
	public GameObject cornerPrefab;
}

// each individual square for the dungeon
public class MapTile
{
	public int m_Type = 0; //Default = 0 , Room Floor = 1, Wall = 2, Corridor Floor 3, Room Corners = 4, 5, 6 , 7
	public int m_Orientation = 0;
	public Room m_Room = null;
}

public class DungeonGenerator : MonoBehaviour
{
	[Tooltip("Spawns the selected prefab at the beginning of the dungeon(usually used for stuff like player spawning).")]
	public GameObject m_StartPrefab;
	[Tooltip("Spawns the selected prefab at the end of the dungeon(usually used for stuff like proceeding to next level).")]
	public GameObject m_ExitPrefab;
	[Tooltip("The floor used in non custom rooms.")]
	public GameObject m_FloorPrefab;
	[Tooltip("The floor used in corridors.")]
	public GameObject corridorFloorPrefab;
	[Tooltip("Default wall being used.")]
	public GameObject m_WallPrefab;
	[Tooltip("Default door being used.")]
	public GameObject m_DoorPrefab;
	[Tooltip("Corner wall being used.")]
	public GameObject m_CornerPrefab;
	[Tooltip("True means it will use the correct orientation for corner prefabs.")]
	public bool m_CornerRotation = false;

	[Tooltip("Decided the maximum ammount of rooms allowed to generate.")]
	public int m_MaximumRoomCount = 10;

	[Tooltip("Maximum gap between rooms. Also affects corridor lengths ")]
	public int m_RoomMargin = 3;

	[Tooltip("If Checked: makes dungeon reset on every time level loads.")]
	public bool m_Generate_on_load = true;
	public int m_MinRoomSize = 5;
	public int m_MaxRoomSize = 10;
	public float m_TileScaling = 1f;
	[Tooltip("Extra set of spawn options")]
	public List<SpawnOption> m_SpawnOptions = new List<SpawnOption>();
	[Tooltip("Implement prefabs for non default rooms.")]
	public List<CustomRoom> m_CustomRooms = new List<CustomRoom>();
	public bool m_MakeIt3d = false;

	private Dungeon dungeon;

	// private class within dungeongenerator so it can't be called from the outside
	private class Dungeon
	{
		public int m_MapSize;
		public int m_MapSizeX;
		public int m_MapSizeY;

		public MapTile[,] m_Map;

		public List<Room> m_Rooms = new List<Room>();

		public Room m_GoalRoom;
		public Room m_StartRoom;

		public int m_MinSize;
		public int m_MaxSize;

		public int m_MaximumRoomCount;
		public int m_RoomMargin;
		public int m_RoomMarginTemp;

		// tile types for ease
		public List<int> m_RoomsAndFloors = new List<int> { 1, 3 };
		public List<int> m_Corners = new List<int> { 4, 5, 6, 7 };
		public List<int> m_Walls = new List<int> { 8, 9, 10, 11 };
		private List<string> m_Directions = new List<string> { "x", "y", "-y", "-x" };

		private int m_CollisionCount = 0;
		private string m_Direction = "set";
		private string m_OldDirection = "set";
		private Room m_LastRoom;

		public void Generate()
		{
			DefineMapSize();

			DefineRooms();

			MakeRooms();

			MakeCorridors();

            int minCropX = CropX();
            int minCropY = CropY();
            foreach (Room room in m_Rooms)
            {
                room.m_X = minCropX;
                room.m_Y = minCropY;
            }

            SetFinalMapSize();

			SetWallTypes();

			SetCornerTypes();

			SetOrientation();

			SetStartEnd();
		}
		//simply decided how big the map should be at the hand of your roommargin maxroomsize and ammount of rooms.
		private void DefineMapSize()
		{
			if (m_RoomMargin < 2)
			{
				m_MapSize = m_MaximumRoomCount * m_MaxSize * 2;
			}
			else
			{
				m_MapSize = (m_MaximumRoomCount * (m_MaxSize + (m_RoomMargin * 2))) + (m_MaximumRoomCount * m_MaximumRoomCount * 2);
			}
			m_Map = new MapTile[m_MapSize, m_MapSize];
			
			for (int x = 0; x < m_MapSize; x++)
			{
				for (int y = 0; y < m_MapSize; y++)
				{
					m_Map[x, y] = new MapTile();
					m_Map[x, y].m_Type = 0;
				}
			}
			m_Rooms = new List<Room>();
		}
		//Defines all the rooms calls a seperate function depending if its the first room or any other.
		private void DefineRooms()
		{
			for (int i = 0; i < m_MaximumRoomCount; i++)
			{
				Room room = new Room();
				if(m_Rooms.Count == 0)
				{
					room = DefineFirstRoom(room);
					m_LastRoom = room;
				}
				else
				{
					room = DefineOtherRooms(room);
				}

				i = DoesItCollide(room, i);
			}
		}
		// defines how big the first room is.
		private Room DefineFirstRoom(Room room)
		{
			room.m_X = (int)Mathf.Floor(m_MapSize / 2f);
			room.m_Y = (int)Mathf.Floor(m_MapSize / 2f);

			room.m_W = Random.Range(m_MinSize, m_MaxSize);
			if (room.m_W % 2 == 0) room.m_W += 1;
			room.m_H = Random.Range(m_MinSize, m_MaxSize);
			if (room.m_H % 2 == 0) room.m_H += 1;
			return room;
		}
		// defines the size of every other room.
		private Room DefineOtherRooms(Room room)
		{
			m_LastRoom = m_Rooms[m_Rooms.Count - 1];


			// This makes the lastRoom variable the room that was not a deadend.
			int lri = 1;
			while (m_LastRoom.m_DeadEnd)
			{
				m_LastRoom = m_Rooms[m_Rooms.Count - lri++];
			}

			if(m_Direction == "set")
			{
				string newRandomDirection = m_Directions[Random.Range(0, m_Directions.Count)];
				m_Direction = newRandomDirection;
				while(m_Direction == m_OldDirection)
				{
					newRandomDirection = m_Directions[Random.Range(0, m_Directions.Count)];
					m_Direction = newRandomDirection;
				}
			}
			m_RoomMarginTemp = Random.Range(0, m_RoomMargin - 1);

			// setting which direction the room needs to point in.
			if(m_Direction == "y")
			{
				room.m_X = m_LastRoom.m_X + m_LastRoom.m_W + Random.Range(3, 5) + m_RoomMarginTemp;
				room.m_Y = m_LastRoom.m_Y;
			}
			else if (m_Direction == "-y")
			{
				room.m_X = m_LastRoom.m_X + m_LastRoom.m_W - Random.Range(3, 5) - m_RoomMarginTemp;
				room.m_Y = m_LastRoom.m_Y;
			}
			else if (m_Direction == "x")
			{
				room.m_Y = m_LastRoom.m_Y + m_LastRoom.m_H + Random.Range(3, 5) + m_RoomMarginTemp;
				room.m_X = m_LastRoom.m_X;
			}
			else if (m_Direction == "-x")
			{
				room.m_Y = m_LastRoom.m_Y + m_LastRoom.m_H - Random.Range(3, 5) - m_RoomMarginTemp;
				room.m_X = m_LastRoom.m_X;
			}

			room.m_W = Random.Range(m_MinSize, m_MaxSize);
			if (room.m_W % 2 == 0) room.m_W += 1;

			room.m_H = Random.Range(m_MinSize, m_MaxSize);
			if (room.m_H % 2 == 0) room.m_H += 1;

			room.m_ConnectedTo = m_LastRoom;
			return room;
		}
		//if it collides with more than 3 rooms the last room becomes a dead end.
		private int DoesItCollide(Room room,int i)
		{
			bool doesCollide = DoesCollide(room);
			if (doesCollide)
			{
				i--;
				m_CollisionCount += 1;
				if (m_CollisionCount > 3)
				{
					//lastRoom.branch = 1;
					m_LastRoom.m_DeadEnd = true;
					m_CollisionCount = 0;
				}
				else
				{
					m_OldDirection = m_Direction;
					m_Direction = "set";
				}
			}
			else
			{
				room.m_RoomId = i;
				m_Rooms.Add(room);
				m_OldDirection = m_Direction;
				m_Direction = "set";
			}
			return i;
		}
		//Checks if the room is colliding with something by comparing its own position on the map with others.
		private bool DoesCollide(Room room)
		{
			// pretty sure it doesnt do anything leaving it commented in here just in case
			//int random_blankliness = 0;

			for (int i = 0; i < m_Rooms.Count; i++)
			{
				Room check = m_Rooms[i];
				if (!((room.m_X + room.m_W /*+ random_blankliness*/ < check.m_X) ||
					 (room.m_X > check.m_X + check.m_W /*+ random_blankliness*/) ||
					 (room.m_Y + room.m_H /*+ random_blankliness*/ < check.m_Y) ||
					 (room.m_Y > check.m_Y + check.m_H /*+ random_blankliness*/)))
					return true;
			}
			return false;
		}
		//Defines what position on the map a room needs to be.
		private void MakeRooms()
		{
            //string output = string.Empty;
            for (int i = 0; i < m_Rooms.Count; i++)
			{
                
				Room room = m_Rooms[i];
				for (int x = room.m_X; x < room.m_X + room.m_W; x++)
				{
					for (int y = room.m_Y; y < room.m_Y + room.m_H; y++)
					{
                        //output += "Room " + i + " " + x + " " + y + "\n";
						m_Map[x, y].m_Type = 1;
						m_Map[x, y].m_Room = room;
					}
				}
			}
            //Debug.Log("makerooms done");
            //Debug.Log(m_Map);
            //Debug.Log(output);
		}

		private void MakeCorridors()
		{
			for (int i = 1; i < m_Rooms.Count; i++)
			{
				CorrectCorridorPlacement(i);
			}
		}

		private void CorrectCorridorPlacement(int i)
		{
			Room roomA = m_Rooms[i];
			Room roomB = m_Rooms[i].m_ConnectedTo;

			//Just to make sure ur not connecting to nothing
			if (roomB != null)
			{
				var pointA = new Room(); //start
				var pointB = new Room();

				//defines the length of the corridor
				pointA.m_X = roomA.m_X + (int)Mathf.Floor(roomA.m_W / 2);
				pointB.m_X = roomB.m_X + (int)Mathf.Floor(roomB.m_W / 2);

				pointA.m_Y = roomA.m_Y + (int)Mathf.Floor(roomA.m_H / 2);
				pointB.m_Y = roomB.m_Y + (int)Mathf.Floor(roomB.m_H / 2);

				// Sets one of the points to the other to synchronise hallway placement.
				if (Mathf.Abs(pointA.m_X - pointB.m_X) > Mathf.Abs(pointA.m_Y - pointB.m_Y))
				{
					if (roomA.m_H > roomB.m_H)
					{
						pointA.m_Y = pointB.m_Y;
					}
					else
					{
						pointB.m_Y = pointA.m_Y;
					}
				}
				else
				{
					if (roomA.m_W > roomB.m_W)
					{
						pointA.m_X = pointB.m_X;
					}
					else
					{
						pointB.m_X = pointA.m_X;
					}
				}

				ImplementCorridors(pointA, pointB);
			}
		}
		// this adds the corridors to the worldmap.
		private void ImplementCorridors(Room pointA, Room pointB)
		{
			while ((pointB.m_X != pointA.m_X) || (pointB.m_Y != pointA.m_Y))
			{
				if (pointB.m_X != pointA.m_X)
				{
					if (pointB.m_X > pointA.m_X)
					{
						pointB.m_X--;
					}
					else
					{
						pointB.m_X++;
					}
				}
				else if (pointB.m_Y != pointA.m_Y)
				{
					if (pointB.m_Y > pointA.m_Y)
					{
						pointB.m_Y--;
					}
					else
					{
						pointB.m_Y++;
					}
				}

				if (m_Map[pointB.m_X, pointB.m_Y].m_Room == null)
				{
					m_Map[pointB.m_X, pointB.m_Y].m_Type = 3;
				}
			}
		}
		//map is created in the middle of the array but it needs to be pushed to the left bottom
		private int CropX()
		{
			int row = 1;
			int min_crop_x = m_MapSize;
			for (int x = 0; x < m_MapSize - 1; x++)
			{
				bool x_empty = true;
				for (int y = 0; y < m_MapSize - 1; y++)
				{
					if (m_Map[x, y].m_Type != 0)
					{
						x_empty = false;
						if (x < min_crop_x)
						{
							min_crop_x = x;
						}
						break;
					}
				}
				if (!x_empty)
				{
					for (int y = 0; y < m_MapSize - 1; y++)
					{
						m_Map[row, y] = m_Map[x, y];
						m_Map[x, y] = new MapTile();
					}
					row += 1;
				}
			}
			return min_crop_x;
		}
		// y crop
		private int CropY()
		{
			int row = 1;
			int min_crop_y = m_MapSize;
			for (int y = 0; y < m_MapSize - 1; y++)
			{
				bool y_empty = true;
				for (int x = 0; x < m_MapSize - 1; x++)
				{
					if (m_Map[x, y].m_Type != 0)
					{
						y_empty = false;
						if (y < min_crop_y)
						{
							min_crop_y = y;
						}
						break;
					}
				}
				if (!y_empty)
				{
					for (int x = 0; x < m_MapSize - 1; x++)
					{
						m_Map[x, row] = m_Map[x, y];
						m_Map[x, y] = new MapTile();
					}
					row += 1;
				}
			}
			return min_crop_y;
		}
		
		private void SetFinalMapSize()
		{
            
            int finalMapSizeY = 0;
            for (int y = 0; y < m_MapSize - 1; y++)
            {
                for (int x = 0; x < m_MapSize - 1; x++)
                {
                    if (m_Map[x, y].m_Type != 0)
                    {
                        finalMapSizeY += 1;
                        break;
                    }
                }
            }

            int finalMapSizeX = 0;
            for (int x = 0; x < m_MapSize - 1; x++)
            {
                for (int y = 0; y < m_MapSize - 1; y++)
                {
                    if (m_Map[x, y].m_Type != 0)
                    {
                        finalMapSizeX += 1;
                        break;
                    }
                }
            }

            finalMapSizeX += 5;
            finalMapSizeY += 5;

            MapTile[,] newMap = new MapTile[finalMapSizeX + 1, finalMapSizeY + 1];
            for (int x = 0; x < finalMapSizeX; x++)
            {
                for (int y = 0; y < finalMapSizeY; y++)
                {
                    newMap[x, y] = m_Map[x, y];
                }
            }
            Debug.Log(m_Map.Length + "mapsizepre");
            m_Map = newMap;
            m_MapSizeX = finalMapSizeX;
            m_MapSizeY = finalMapSizeY;
            Debug.Log(m_MapSizeX + "mapsizeXpost");
            Debug.Log(m_MapSizeY + "mapsizeXpost");
            Debug.Log(m_Map.Length + "mapsizeXpost");
        }

		private void SetWallTypes()
		{
			for (int x = 0; x < m_MapSizeX - 1; x++)
			{
				for (int y = 0; y < m_MapSizeY - 1; y++)
				{
					if (m_Map[x, y].m_Type == 0)
					{
						if (m_Map[x + 1, y].m_Type == 1 || m_Map[x + 1, y].m_Type == 3)
						{ //west
							m_Map[x, y].m_Type = 11;
							m_Map[x, y].m_Room = m_Map[x + 1, y].m_Room;
						}
						if (x > 0)
						{
							if (m_Map[x - 1, y].m_Type == 1 || m_Map[x - 1, y].m_Type == 3)
							{ //east
								m_Map[x, y].m_Type = 9;
								m_Map[x, y].m_Room = m_Map[x - 1, y].m_Room;

							}
						}

						if (m_Map[x, y + 1].m_Type == 1 || m_Map[x, y + 1].m_Type == 3)
						{ //south
							m_Map[x, y].m_Type = 10;
							m_Map[x, y].m_Room = m_Map[x, y + 1].m_Room;

						}

						if (y > 0)
						{
							if (m_Map[x, y - 1].m_Type == 1 || m_Map[x, y - 1].m_Type == 3)
							{ //north
								m_Map[x, y].m_Type = 8;
								m_Map[x, y].m_Room = m_Map[x, y - 1].m_Room;

							}
						}
					}
				}
			}
		}

		private void SetCornerTypes()
		{
			for (int x = 0; x < m_MapSizeX - 1; x++)
			{
				for (int y = 0; y < m_MapSizeY - 1; y++)
				{
					if (m_Walls.Contains(m_Map[x, y + 1].m_Type) && m_Walls.Contains(m_Map[x + 1, y].m_Type) && m_RoomsAndFloors.Contains(m_Map[x + 1, y + 1].m_Type))
					{ //north
						m_Map[x, y].m_Type = 4;
						m_Map[x, y].m_Room = m_Map[x + 1, y + 1].m_Room;
					}
					if (y > 0)
					{
						if (m_Walls.Contains(m_Map[x + 1, y].m_Type) && m_Walls.Contains(m_Map[x, y - 1].m_Type) && m_RoomsAndFloors.Contains(m_Map[x + 1, y - 1].m_Type))
						{ //north
							m_Map[x, y].m_Type = 5;
							m_Map[x, y].m_Room = m_Map[x + 1, y - 1].m_Room;

						}
					}
					if (x > 0)
					{
						if (m_Walls.Contains(m_Map[x - 1, y].m_Type) && m_Walls.Contains(m_Map[x, y + 1].m_Type) && m_RoomsAndFloors.Contains(m_Map[x - 1, y + 1].m_Type))
						{ //north
							m_Map[x, y].m_Type = 7;
							m_Map[x, y].m_Room = m_Map[x - 1, y + 1].m_Room;

						}
					}
					if (x > 0 && y > 0)
					{
						if (m_Walls.Contains(m_Map[x - 1, y].m_Type) && m_Walls.Contains(m_Map[x, y - 1].m_Type) && m_RoomsAndFloors.Contains(m_Map[x - 1, y - 1].m_Type))
						{ //north
							m_Map[x, y].m_Type = 6;
							m_Map[x, y].m_Room = m_Map[x - 1, y - 1].m_Room;

						}
					}
					/* door corners --- a bit problematic in this version */
					if (m_Map[x, y].m_Type == 3)
					{
						if (m_Map[x + 1, y].m_Type == 1)
						{
							m_Map[x, y + 1].m_Type = 11;
							m_Map[x, y - 1].m_Type = 11;
						}
						else if (m_Map[x - 1, y].m_Type == 1)
						{
							m_Map[x, y + 1].m_Type = 9;
							m_Map[x, y - 1].m_Type = 9;
						}
					}

				}
			}
		}

		private void SetOrientation()
		{
			for (int x = 0; x < m_MapSizeX - 1; x++)
			{
				for (int y = 0; y < m_MapSizeY - 1; y++)
				{
					if (m_Map[x, y].m_Type == 3)
					{
						bool cw = m_Map[x, y + 1].m_Type == 3;
						bool ce = m_Map[x, y - 1].m_Type == 3;
						bool cn = m_Map[x + 1, y].m_Type == 3;
						bool cs = m_Map[x - 1, y].m_Type == 3;
						if (cw || ce)
						{
							m_Map[x, y].m_Orientation = 1;
						}
						else if (cn || cs)
						{
							m_Map[x, y].m_Orientation = 2;
						}
					}
				}
			}

		}

		private void SetStartEnd()
		{
			//find far far away room
			m_GoalRoom = m_Rooms[m_Rooms.Count - 1];
			if (m_GoalRoom != null)
			{
				m_GoalRoom.m_X = m_GoalRoom.m_X + (m_GoalRoom.m_W / 2);
				m_GoalRoom.m_Y = m_GoalRoom.m_Y + (m_GoalRoom.m_H / 2);
			}
			//starting point
			m_StartRoom = m_Rooms[0];
			m_StartRoom.m_X = m_StartRoom.m_X + (m_StartRoom.m_W / 2);
			m_StartRoom.m_Y = m_StartRoom.m_Y + (m_StartRoom.m_H / 2);
		}

	}

	
	// destroys the old dungeon
	public void ClearOldDungeon(bool immediate = false)
	{
		int childs = transform.childCount;
		for (var i = childs - 1; i >= 0; i--)
		{
			if (immediate)
			{
				DestroyImmediate(transform.GetChild(i).gameObject);
			}
			else
			{
				Destroy(transform.GetChild(i).gameObject);
			}
		}
	}

	public void Spawn()
	{
		// used to instantiate dungeon here now i do it as a global member to not make it a parameter in every function.
		dungeon = new Dungeon();

		dungeon.m_MinSize = m_MinRoomSize;
		dungeon.m_MaxSize = m_MaxRoomSize;
		dungeon.m_MaximumRoomCount = m_MaximumRoomCount;
		dungeon.m_RoomMargin = m_RoomMargin;

		dungeon.Generate();

		InstantiateDungeon();

		SetStartEnd();

		//Spawn Objects;
		List<SpawnList> spawnedObjectLocations = new List<SpawnList>();

        spawnedObjectLocations = SetObjectSpawnLocations(spawnedObjectLocations);

        spawnedObjectLocations = CreateDoors(spawnedObjectLocations);

        int objectCountToSpawn = 0;
        foreach (SpawnOption objectToSpawn in m_SpawnOptions)
        {
            objectCountToSpawn = Random.Range(objectToSpawn.m_MinSpawnCount, objectToSpawn.m_MaxSpawnCount);
            while(objectCountToSpawn > 0)
            {
                bool created = false;
                
                for (int i = 0; i < spawnedObjectLocations.Count; i++)
                {
                    bool createHere = false;
                    createHere = CheckToCreateObject(createHere, spawnedObjectLocations, i,objectToSpawn);
                    if (createHere)
                    {
                        created = CreateObjects(created, spawnedObjectLocations,i, objectToSpawn);
                        objectCountToSpawn--;
                        break;
                    }
                }
                if (!created)
                {
                    objectCountToSpawn--;
                }
                
            }
        }

    }
	private void InstantiateDungeon()
	{
		for (int y = 0; y < dungeon.m_MapSizeY; y++)
		{
			for (int x = 0; x < dungeon.m_MapSizeX; x++)
			{
				int tile = dungeon.m_Map[x, y].m_Type;
				int orientation = dungeon.m_Map[x, y].m_Type;
				GameObject createdTile;
				Vector3 tileLocation = SetTileLocation(x, y, tile);

				createdTile = null;
				if (tile == 1)
				{
					createdTile = CreateFloor(x, y, tileLocation);
				}

				if (dungeon.m_Walls.Contains(tile))
				{
					createdTile = CreateWalls(x, y, tileLocation, tile);
				}

				if (tile == 3)
				{
					createdTile = CreateCorridorFloors(x, y, tileLocation, orientation);
				}

				if (dungeon.m_Corners.Contains(tile))
				{
					createdTile = CreateCornerWalls(x, y, tileLocation, tile);
				}

				if (createdTile)
				{
					createdTile.transform.parent = transform;
				}
			}
		}
	}

	private Vector3 SetTileLocation(int x, int y, int tile)
	{
		Vector3 tileLocation;
		if (!m_MakeIt3d)
		{
			tileLocation = new Vector3(x * m_TileScaling, y * m_TileScaling, 0);
		}
		else
		{
			tileLocation = new Vector3(x * m_TileScaling, 0, y * m_TileScaling);
			if (tile == 2 || tile == 8 || tile == 9 || tile == 10 || tile == 11 || tile == 4 || tile == 5 || tile == 6 || tile == 7 || tile == 8)
			{
				tileLocation.y = 0.6f;
			}
		}
		return tileLocation;
	}

	private GameObject CreateFloor(int x, int y, Vector3 tileLocation)
	{
		GameObject floorPrefabToUse = m_FloorPrefab;
		Room room = dungeon.m_Map[x, y].m_Room;
		if (room != null)
		{
			foreach (CustomRoom customroom in m_CustomRooms)
			{
				if (customroom.roomId == room.m_RoomId)
				{
					floorPrefabToUse = customroom.floorPrefab;
					break;
				}
			}
		}
		return GameObject.Instantiate(floorPrefabToUse, tileLocation, Quaternion.identity) as GameObject; ;
	}

	private GameObject CreateWalls(int x, int y, Vector3 tileLocation, int tile)
	{
		GameObject wallPrefabToUse = m_WallPrefab;
		Room room = dungeon.m_Map[x, y].m_Room;
		if(room != null)
		{
			foreach (CustomRoom customroom in m_CustomRooms)
			{
				if (customroom.roomId == room.m_RoomId)
				{
					wallPrefabToUse = customroom.wallPrefab;
					break;
				}
			}
		}

		GameObject createdTile = GameObject.Instantiate(wallPrefabToUse, new Vector3(tileLocation.x, tileLocation.y, tileLocation.z), Quaternion.identity) as GameObject;

		if (!m_MakeIt3d)
		{
			createdTile.transform.Rotate(Vector3.forward * (-90 * (tile - 4)));
		}
		else
		{
			createdTile.transform.Rotate(Vector3.up * (-90 * (tile - 4)));
		}
		return createdTile;
	}

	private GameObject CreateCorridorFloors(int x, int y, Vector3 tileLocation, int orientation)
	{
		GameObject createdTile;
		if (corridorFloorPrefab)
		{
			createdTile = GameObject.Instantiate(corridorFloorPrefab, tileLocation, Quaternion.identity) as GameObject;
		}
		else
		{
			createdTile = GameObject.Instantiate(m_FloorPrefab, tileLocation, Quaternion.identity) as GameObject;
		}

		if (orientation == 1 && m_MakeIt3d)
		{
			createdTile.transform.Rotate(Vector3.up * (-90));
		}
		return createdTile;
	}

	private GameObject CreateCornerWalls(int x, int y, Vector3 tileLocation, int tile)
	{
		GameObject cornerPrefabToUse = m_CornerPrefab;
		Room room = dungeon.m_Map[x, y].m_Room;
		GameObject createdTile;
		if (room != null)
		{
			foreach (CustomRoom customroom in m_CustomRooms)
			{
				if (customroom.roomId == room.m_RoomId)
				{
					cornerPrefabToUse = customroom.cornerPrefab;
					break;
				}
			}
		}

		if (cornerPrefabToUse)
		{
			createdTile = GameObject.Instantiate(cornerPrefabToUse, new Vector3(tileLocation.x, tileLocation.y, tileLocation.z), Quaternion.identity) as GameObject;
			if (m_CornerRotation)
			{
				if (!m_MakeIt3d)
				{
					createdTile.transform.Rotate(Vector3.forward * (-90 * (tile - 4)));
				}
				else
				{
					createdTile.transform.Rotate(Vector3.up * (-90 * (tile - 4)));
				}
			}
		}
		else
		{
			createdTile = GameObject.Instantiate(m_WallPrefab, new Vector3(tileLocation.x, tileLocation.y, tileLocation.z), Quaternion.identity) as GameObject;
		}
		return createdTile;
	}

	private void SetStartEnd()
	{
		GameObject endPoint;
		GameObject startPoint;
		if (!m_MakeIt3d)
		{
			endPoint = GameObject.Instantiate(m_ExitPrefab, new Vector3(dungeon.m_GoalRoom.m_X * m_TileScaling, dungeon.m_GoalRoom.m_Y * m_TileScaling, 0), Quaternion.identity) as GameObject;
			startPoint = GameObject.Instantiate(m_StartPrefab, new Vector3(dungeon.m_StartRoom.m_X * m_TileScaling, dungeon.m_StartRoom.m_Y * m_TileScaling, 0), Quaternion.identity) as GameObject;
		}
		else
		{
			endPoint = GameObject.Instantiate(m_ExitPrefab, new Vector3(dungeon.m_GoalRoom.m_X * m_TileScaling,0, dungeon.m_GoalRoom.m_Y * m_TileScaling), Quaternion.identity) as GameObject;
			startPoint = GameObject.Instantiate(m_StartPrefab, new Vector3(dungeon.m_StartRoom.m_X * m_TileScaling,0, dungeon.m_StartRoom.m_Y * m_TileScaling), Quaternion.identity) as GameObject;
		}

		endPoint.transform.parent = transform;
		startPoint.transform.parent = transform;
	}

	private List<SpawnList> SetObjectSpawnLocations(List<SpawnList> spawnedObjectLocations)
	{
		for (int x = 0; x < dungeon.m_MapSizeX; x++)
		{
			for (int y = 0; y < dungeon.m_MapSizeY; y++)
			{
				if (dungeon.m_Map[x, y].m_Type == 1 &&
						((dungeon.m_StartRoom != dungeon.m_Map[x, y].m_Room && dungeon.m_GoalRoom != dungeon.m_Map[x, y].m_Room) || m_MaximumRoomCount <= 3))
				{
					SpawnList location = new SpawnList();

					location.m_X = x;
					location.m_Y = y;
					if (dungeon.m_Walls.Contains(dungeon.m_Map[x + 1, y].m_Type))
					{
						location.m_ByWall = true;
						location.m_WallLocation = "S";
					}
					else if (dungeon.m_Walls.Contains(dungeon.m_Map[x - 1, y].m_Type))
					{
						location.m_ByWall = true;
						location.m_WallLocation = "N";
					}
					else if (dungeon.m_Walls.Contains(dungeon.m_Map[x, y + 1].m_Type))
					{
						location.m_ByWall = true;
						location.m_WallLocation = "W";
					}
					else if (dungeon.m_Walls.Contains(dungeon.m_Map[x, y - 1].m_Type))
					{
						location.m_ByWall = true;
						location.m_WallLocation = "E";
					}

					if (dungeon.m_Map[x + 1, y].m_Type == 3 || dungeon.m_Map[x - 1, y].m_Type == 3 || dungeon.m_Map[x, y + 1].m_Type == 3 || dungeon.m_Map[x, y - 1].m_Type == 3)
					{
						location.m_ByCorridor = true;
					}
					if (dungeon.m_Map[x + 1, y + 1].m_Type == 3 || dungeon.m_Map[x - 1, y - 1].m_Type == 3 || dungeon.m_Map[x - 1, y + 1].m_Type == 3 || dungeon.m_Map[x + 1, y - 1].m_Type == 3)
					{
						location.m_ByCorridor = true;
					}
					location.m_Room = dungeon.m_Map[x, y].m_Room;

					int roomCenterX = (int)Mathf.Floor(location.m_Room.m_W / 2) + location.m_Room.m_X;
					int roomCenterY = (int)Mathf.Floor(location.m_Room.m_H / 2) + location.m_Room.m_Y;

					if (x == roomCenterX + 1 && y == roomCenterY + 1)
					{
						location.m_InTheMiddle = true;
					}
					spawnedObjectLocations.Add(location);
				}
				else if (dungeon.m_Map[x, y].m_Type == 3)
				{
					var location = new SpawnList();
					location.m_X = x;
					location.m_Y = y;

					if (dungeon.m_Map[x + 1, y].m_Type == 1)
					{
						location.m_ByCorridor = true;
						location.m_AsDoor = 4;
						location.m_Room = dungeon.m_Map[x + 1, y].m_Room;

						spawnedObjectLocations.Add(location);
					}
					else if (dungeon.m_Map[x - 1, y].m_Type == 1)
					{
						location.m_ByCorridor = true;
						location.m_AsDoor = 2;
						location.m_Room = dungeon.m_Map[x - 1, y].m_Room;

						spawnedObjectLocations.Add(location);
					}
					else if (dungeon.m_Map[x, y + 1].m_Type == 1)
					{
						location.m_ByCorridor = true;
						location.m_AsDoor = 1;
						location.m_Room = dungeon.m_Map[x, y + 1].m_Room;

						spawnedObjectLocations.Add(location);
					}
					else if (dungeon.m_Map[x, y - 1].m_Type == 1)
					{
						location.m_ByCorridor = true;
						location.m_AsDoor = 3;
						location.m_Room = dungeon.m_Map[x, y - 1].m_Room;

						spawnedObjectLocations.Add(location);
					}
				}
			}
		}

        for (int i = 0; i < spawnedObjectLocations.Count; i++)
        {
            SpawnList temp = spawnedObjectLocations[i];
            int randomIndex = Random.Range(i, spawnedObjectLocations.Count);
            spawnedObjectLocations[i] = spawnedObjectLocations[randomIndex];
            spawnedObjectLocations[randomIndex] = temp;
        }
        return spawnedObjectLocations;
    }

    private List<SpawnList> CreateDoors(List<SpawnList> spawnedObjectLocations)
    {
        if (m_DoorPrefab)
        {
            for (int i = 0; i < spawnedObjectLocations.Count; i++)
            {
                if(spawnedObjectLocations[i].m_AsDoor > 0)
                {
                    GameObject newObject;
                    SpawnList spawnLocation = spawnedObjectLocations[i];

                    GameObject doorPrefabToUse = m_DoorPrefab;
                    Room room = spawnLocation.m_Room;
                    if(room != null)
                    {
                        foreach (CustomRoom customroom in m_CustomRooms)
                        {
                            if (customroom.roomId == room.m_RoomId)
                            {
                                doorPrefabToUse = customroom.doorPrefab;
                                break;
                            }
                        }
                    }

                    if (!m_MakeIt3d)
                    {
                        newObject = GameObject.Instantiate(doorPrefabToUse, new Vector3(spawnLocation.m_X * m_TileScaling, spawnLocation.m_Y * m_TileScaling, 0), Quaternion.identity) as GameObject;
                    }
                    else
                    {
                        newObject = GameObject.Instantiate(doorPrefabToUse, new Vector3(spawnLocation.m_X * m_TileScaling, 0, spawnLocation.m_Y * m_TileScaling), Quaternion.identity) as GameObject;
                    }

                    if (!m_MakeIt3d)
                    {
                        newObject.transform.Rotate(Vector3.forward * (-90 * (spawnedObjectLocations[i].m_AsDoor - 1)));
                    }
                    else
                    {
                        newObject.transform.Rotate(Vector3.up * (-90 * (spawnedObjectLocations[i].m_AsDoor - 1)));
                    }

                    newObject.transform.parent = transform;
                    spawnedObjectLocations[i].m_SpawnedObject = newObject;
                }
            }
        }
        return spawnedObjectLocations;
    }

    private bool CheckToCreateObject(bool createHere, List<SpawnList> spawnedObjectLocations, int i, SpawnOption objectToSpawn)
    {
        if (!spawnedObjectLocations[i].m_SpawnedObject && !spawnedObjectLocations[i].m_ByCorridor)
        {
            if(objectToSpawn.m_SpawnRoom > m_MaximumRoomCount)
            {
                objectToSpawn.m_SpawnRoom = 0;
            }
            if(objectToSpawn.m_SpawnRoom == 0)
            {
                if (objectToSpawn.m_SpawnByWall)
                {
                    if (spawnedObjectLocations[i].m_ByWall)
                    {
                        createHere = true;
                    }
                }
                else if (objectToSpawn.m_SpawnInMiddle)
                {
                    if (spawnedObjectLocations[i].m_InTheMiddle)
                    {
                        createHere = true;
                    }
                }
                else
                {
                    createHere = true;
                }
            }
            else
            {
                if(spawnedObjectLocations[i].m_Room.m_RoomId == objectToSpawn.m_SpawnRoom)
                {
                    if (objectToSpawn.m_SpawnByWall)
                    {
                        if (spawnedObjectLocations[i].m_ByWall)
                        {
                            createHere = true;
                        }
                    }
                    else
                    {
                        createHere = true;
                    }
                }
            }
        }
        return createHere;
    }
    
    private bool CreateObjects(bool created, List<SpawnList> spawnedObjectLocations,int i, SpawnOption objectToSpawn)
    {
        SpawnList spawnLocation = spawnedObjectLocations[i];
        GameObject newObject;
        Quaternion spawnRotation = Quaternion.identity;

        if (!m_MakeIt3d)
        {
            newObject = GameObject.Instantiate(objectToSpawn.gameObject, new Vector3(spawnLocation.m_X * m_TileScaling, spawnLocation.m_Y * m_TileScaling, 0), spawnRotation) as GameObject;
        }
        else
        {
            if (spawnLocation.m_ByWall)
            {
                if(spawnLocation.m_WallLocation == "S")
                {
                    spawnRotation = Quaternion.Euler(new Vector3(0, 270, 0));
                }
                else if(spawnLocation.m_WallLocation == "N")
                {
                    spawnRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                }
                else if (spawnLocation.m_WallLocation == "W")
                {
                    spawnRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                }
                else if(spawnLocation.m_WallLocation == "E")
                {
                    spawnRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }
            }
            else
            {
                if (objectToSpawn.m_SpawnRotated)
                {
                    spawnRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
                }
                else
                {
                    spawnRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 2) * 90, 0));
                }
            }

            newObject = GameObject.Instantiate(objectToSpawn.gameObject, new Vector3(spawnLocation.m_X * m_TileScaling, 0 + objectToSpawn.m_HeightFix, spawnLocation.m_Y * m_TileScaling), spawnRotation) as GameObject;
        }

        newObject.transform.parent = transform;
        spawnedObjectLocations[i].m_SpawnedObject = newObject;

        created = true;
        return created;
    }

    private void Start()
    {
        if (m_Generate_on_load)
        {
            ClearOldDungeon();
            Spawn();

        }
    }
}
