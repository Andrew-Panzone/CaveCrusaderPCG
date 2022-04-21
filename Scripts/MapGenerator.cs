using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapGenerator : MonoBehaviour {
    public int width;
    public int height;
    public string seed = "";
    bool[,] map;
    Mesh mesh;
	public GameObject playerSpawnPoint;
	public GameObject playerPrefab;
	GameObject bossPrefab;
	public GameObject[] bossPrefabs;
	public GameObject[] minionPrefabs;
	System.Random rand;

	public static int bossCount = 0;

    void Start() {
        genMap();
        for (int i = 0; i < 6; i++) {
            smooth();
        }
        List<List<Posn>> roomRegions = GetRegions(false);
        List<Room> rooms = new List<Room>();
        foreach (List<Posn> roomRegion in roomRegions) {
			rooms.Add(new Room(roomRegion, map));
		}
        rooms.Sort ();
		rooms[0].isMainRoom = true;
		rooms[0].isAccessibleFromMainRoom = true;
		// spawn bosses
		
		ConnectClosestRooms(rooms);
		FindFirstFalse();
        createMesh();
		SpawnPlayer();
		GetBossRooms(rooms);

	}

    void createMesh() {
        Vector3[] vertices = new Vector3[(width+1)*(height+1)];
        Vector2[] uvs = new Vector2[(width+1)*(height+1)];
        int i = 0;
        for(int x = -width/2; x <= width/2; x++) {
            for(int y = -height/2; y <= height/2; y++) {
                vertices[i] = new Vector3(x, y, 0);
                uvs[i] = new Vector2(x/(float)width, y/(float)height);
                i++;
            }
        }
        int[] triangles = new int[width*height*6];
        i = 0;
        int n = 0;
        for (int x = 0; x < width; x++) {
            for(int y  = 0; y < height; y++) {
                if (map[x,y]) {
                    triangles[n] = i;
                    triangles[n + 2] = i + height + 1;
                    triangles[n + 1] = i + 1;
                    triangles[n + 4] = i + 1;
                    triangles[n + 3] = i + height + 1;
                    triangles[n + 5] = i + height + 2;
                }
                
                i++;
                n += 6;
            }
            i++;
        }
        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        GetComponent<MeshFilter>().mesh = mesh;
        MeshCollider collider = gameObject.GetComponent<MeshCollider>();
        collider.sharedMesh = mesh;
    }

    void genMap() {
		int randomInt = UnityEngine.Random.Range(0, 9999999);
		if(seed != "") {
			rand = new System.Random(seed.GetHashCode());
		} else {
			rand = new System.Random(randomInt.GetHashCode());
		}
        map = new bool[width,height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (x == 0 || y == 0 || x == width-1 || y == height-1) {
                    map[x,y] = true;
                } else {
                    map[x,y] = rand.Next(0,100) < 50;
                }
            }
        }
    }

    void smooth() {
        bool[,] newMap = new bool[width,height];
        for (int x = 1; x < width-1; x++) {
            for (int y = 1; y < height-1; y++) {
                int numNeighbors = 0;
                if (map[x-1,y-1]) {
                    numNeighbors++;
                }
                if (map[x-1,y]) {
                    numNeighbors++;
                }
                if (map[x-1,y+1]) {
                    numNeighbors++;
                }
                if (map[x,y-1]) {
                    numNeighbors++;
                }
                if (map[x,y+1]) {
                    numNeighbors++;
                }
                if (map[x+1,y-1]) {
                    numNeighbors++;
                }
                if (map[x+1,y]) {
                    numNeighbors++;
                }
                if (map[x+1,y+1]) {
                    numNeighbors++;
                }
                if (numNeighbors > 4) {
                    map[x,y] = true;
                } else if (numNeighbors < 4) {
                    map[x,y] = false;
                }
            }
        }
    }

    struct Posn {
		public int x;
		public int y;

		public Posn(int nx, int ny) {
			x = nx;
			y = ny;
		}
	}

    List<List<Posn>> GetRegions(bool tileType) {
		List<List<Posn>> regions = new List<List<Posn>> ();
		bool[,] mapFlags = new bool[width,height];

		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				if (!mapFlags[x,y] && map[x,y] == tileType) {
					List<Posn> newRegion = GetRegionTiles(x,y);
					regions.Add(newRegion);

					foreach (Posn tile in newRegion) {
						mapFlags[tile.x, tile.y] = true;
					}
				}
			}
		}

		return regions;
	}

    List<Posn> GetRegionTiles(int startX, int startY) {
		List<Posn> tiles = new List<Posn> ();
		int[,] mapFlags = new int[width,height];
		bool tileType = map[startX, startY];

		Queue<Posn> queue = new Queue<Posn> ();
		queue.Enqueue (new Posn (startX, startY));
		mapFlags [startX, startY] = 1;

		while (queue.Count > 0) {
			Posn tile = queue.Dequeue();
			tiles.Add(tile);

			for (int x = tile.x - 1; x <= tile.x + 1; x++) {
				for (int y = tile.y - 1; y <= tile.y + 1; y++) {
					if (x > 0 && x < width && y > 0 && y < width && (y == tile.y || x == tile.x)) {
						if (mapFlags[x,y] == 0 && map[x,y] == tileType) {
							mapFlags[x,y] = 1;
							queue.Enqueue(new Posn(x,y));
						}
					}
				}
			}
		}

		return tiles;
	}

    void ConnectClosestRooms(List<Room> allRooms) {

		List<Room> roomListA = new List<Room> ();
		List<Room> roomListB = new List<Room> ();

		foreach (Room room in allRooms) {
			if (room.isAccessibleFromMainRoom) {
				roomListB.Add (room);
			} else {
				roomListA.Add (room);
			}
		}
		

		int bestDistance = 0;
		Posn bestTileA = new Posn ();
		Posn bestTileB = new Posn ();
		Room bestRoomA = new Room ();
		Room bestRoomB = new Room ();
		bool found = false;

		foreach (Room roomA in roomListA) {

			foreach (Room roomB in roomListB) {
				if (roomA == roomB || roomA.IsConnected(roomB)) {
					continue;
				}
			
				for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA ++) {
					for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB ++) {
						Posn tileA = roomA.edgeTiles[tileIndexA];
						Posn tileB = roomB.edgeTiles[tileIndexB];
						int distanceBetweenRooms = (int)(Mathf.Pow (tileA.x-tileB.x,2) + Mathf.Pow (tileA.y-tileB.y,2));

						if (distanceBetweenRooms < bestDistance || !found) {
							bestDistance = distanceBetweenRooms;
							found = true;
							bestTileA = tileA;
							bestTileB = tileB;
							bestRoomA = roomA;
							bestRoomB = roomB;
						}
					}
				}
			}
		}

		if (found) {
			CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
			ConnectClosestRooms(allRooms);
		}
	}

	void CreatePassage(Room roomA, Room roomB, Posn tileA, Posn tileB) {
		Room.ConnectRooms (roomA, roomB);
		List<Posn> line = GetLine (tileA, tileB);
		foreach (Posn c in line) {
			DrawCircle(c,1);
		}
	}

	void DrawCircle(Posn c, int r) {
		for (int x = -r; x <= r; x++) {
			for (int y = -r; y <= r; y++) {
				if (x*x + y*y <= r*r) {
					int drawX = c.x + x;
					int drawY = c.y + y;
					if (drawX > 0 && drawX < width && drawY > 0 && drawY < height) {
						map[drawX,drawY] = false;
					}
				}
			}
		}
	}


	List<Posn> GetLine(Posn from, Posn to) {
		List<Posn> line = new List<Posn> ();

		int x = from.x;
		int y = from.y;

		int dx = to.x - from.x;
		int dy = to.y - from.y;

		bool inverted = false;
		int step = Math.Sign (dx);
		int gradientStep = Math.Sign (dy);

		int longest = Mathf.Abs (dx);
		int shortest = Mathf.Abs (dy);

		if (longest < shortest) {
			inverted = true;
			longest = Mathf.Abs(dy);
			shortest = Mathf.Abs(dx);

			step = Math.Sign (dy);
			gradientStep = Math.Sign (dx);
		}

		int gradientAccumulation = longest / 2;
		for (int i =0; i < longest; i ++) {
			line.Add(new Posn(x,y));

			if (inverted) {
				y += step;
			}
			else {
				x += step;
			}

			gradientAccumulation += shortest;
			if (gradientAccumulation >= longest) {
				if (inverted) {
					x += gradientStep;
				}
				else {
					y += gradientStep;
				}
				gradientAccumulation -= longest;
			}
		}

		return line;
	}

	// Get Boss rooms
	void GetBossRooms(List<Room> allRooms)
	{
		List<Room> bossRooms = new List<Room>();


		foreach (Room room in allRooms)
		{
			bossRooms.Add(room);
		}

		GenBoss(bossRooms);
	}

	//Generate enemies
	void GenBoss(List<Room> bossRooms)
	{

		var rand = new System.Random();
		foreach (Room room in bossRooms)
		{
			int pos = rand.Next(room.roomSize);
			Vector3 bossPos = new Vector3(-width/2 + room.tiles[pos].x + 0.5f, -height/2 + room.tiles[pos].y + 20.5f, 0);

			bossPrefab = bossPrefabs[UnityEngine.Random.Range(0, bossPrefabs.Length)];

			int index = System.Array.IndexOf(bossPrefabs, bossPrefab);

			GameObject boss = Instantiate(bossPrefab, bossPos, Quaternion.identity)
		   as GameObject;

		   SpawnMinionsAroundBoss(index, bossPos);

		   bossCount++;
		}
	}

	void SpawnMinionsAroundBoss(int index, Vector3 bossPos)
	{
		int rand = UnityEngine.Random.Range(3, 10);

		for(int i = 0; i <= rand; i++)
        {
			Vector3 minionPos = new Vector3(bossPos.x + GetRandomValue(), bossPos.y + GetRandomValue(), 0);


			GameObject minion = Instantiate(minionPrefabs[index], minionPos, Quaternion.identity)
				as GameObject;

            if (DetectCollision(minionPos) > 1){
				GameObject.Destroy(minion);
            }
        }

	}

	float GetRandomValue()
	{
		float rand = UnityEngine.Random.value;
		if (rand <= .3f)
			return UnityEngine.Random.Range(-1.0f ,0);
		if (rand <= .6f)
			return UnityEngine.Random.Range(0, 1.0f);
		if(rand<= .7f)
			return UnityEngine.Random.Range(-2.0f, -1.0f);
		if(rand<= .8f)
			return UnityEngine.Random.Range(1.0f, 2);
		if(rand <= .9f)
			return UnityEngine.Random.Range(-4.0f, -2);
		return UnityEngine.Random.Range(2.0f, 4);
	}

	private int DetectCollision(Vector3 pos) {
		Collider[] hitColliders = Physics.OverlapSphere(pos, 0.6f);
		return hitColliders.Length;
	}



	class Room : IComparable<Room> {
		public List<Posn> tiles;
		public List<Posn> edgeTiles;
		public List<Room> connectedRooms;
		public int roomSize;
		public bool isAccessibleFromMainRoom;
		public bool isMainRoom;

		public Room() {
		}

		public Room(List<Posn> roomTiles, bool[,] map) {
			tiles = roomTiles;
			roomSize = tiles.Count;
			connectedRooms = new List<Room>();

			edgeTiles = new List<Posn>();
			foreach (Posn tile in tiles) {
				for (int x = tile.x-1; x <= tile.x+1; x++) {
					for (int y = tile.y-1; y <= tile.y+1; y++) {
						if (x == tile.x || y == tile.y) {
							if (map[x,y]) {
								edgeTiles.Add(tile);
							}
						}
					}
				}
			}
		}

		public void SetAccessibleFromMainRoom() {
			if (!isAccessibleFromMainRoom) {
				isAccessibleFromMainRoom = true;
				foreach (Room connectedRoom in connectedRooms) {
					connectedRoom.SetAccessibleFromMainRoom();
				}
			}
		}

		public static void ConnectRooms(Room roomA, Room roomB) {
			if (roomA.isAccessibleFromMainRoom) {
				roomB.SetAccessibleFromMainRoom ();
			} else if (roomB.isAccessibleFromMainRoom) {
				roomA.SetAccessibleFromMainRoom();
			}
			roomA.connectedRooms.Add (roomB);
			roomB.connectedRooms.Add (roomA);
		}

		public bool IsConnected(Room otherRoom) {
			return connectedRooms.Contains(otherRoom);
		}

		public int CompareTo(Room otherRoom) {
			return otherRoom.roomSize.CompareTo (roomSize);
		}
	}

	void FindFirstFalse()
	{
		for(int i = 0; i < height; i++)
		{
			for(int j = 0; j < width; j++)
			{
				if(!map[j,i])
				{
					GenerateSpawnRoom(j,i);
					return;
				}
			}
		}
	}

	void GenerateSpawnRoom(int x, int y)
	{
		playerSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
		height += 20;

		bool[,] nMap = new bool[width, height];
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (j < 20) {
					nMap[i,j] = !(x == i) || j == 0;
				} else {
					if (j < y + 20 && i == x) {
						nMap[i,j] = false;
					} else {
						nMap[i,j] = map[i,j-20];
					}
				}
			}
		}
		map = nMap;

		playerSpawnPoint.transform.position = new Vector3(x-(width/2), 10 - (height/2), 0);
	}

	void SpawnPlayer()
	{
		GameObject player = Instantiate(playerPrefab, playerSpawnPoint.transform.position, playerSpawnPoint.transform.rotation)
                as GameObject;
	}
}