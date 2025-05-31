using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] GameObject roomPrefab;
    [SerializeField] GameObject enemySpawnerPrefab;
    [SerializeField] GameObject bossPrefab;
    [SerializeField] GameObject treasurePrefab;
    //[SerializeField] GameObject player;
    [SerializeField] int minRooms = 6;
    [SerializeField] int maxRooms = 10;

    int roomWidth = 20;
    int roomHeight = 21;

    int gridSizeX = 10;
    int gridSizeY = 10;

    public int currentLevel = 1;
    public int maxLevel = 5;

    private List<GameObject> roomObjects = new List<GameObject>();
    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();
    private Vector2Int farthestRoomIndex;
    private GameObject stairRoomObject;
    private GameObject player;
    private bool bossDefeated = false;
    private int[,] roomGrid;
    private int roomCount;
    private bool generationComplete = false;

    private Dictionary<Vector2Int, GameObject> roomSpawners = new Dictionary<Vector2Int, GameObject>();

    private void Start()
    {
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue = new Queue<Vector2Int>();

        StartCoroutine(DelayedStart());

        //Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        //StartRoomGenerationFromRoom(initialRoomIndex);
    }

    private void Update()
    {
        if (roomQueue.Count > 0 && roomCount < maxRooms && !generationComplete)
        {
            Vector2Int roomIndex = roomQueue.Dequeue();
            int gridX = roomIndex.x;
            int gridY = roomIndex.y;

            TryGenerateRoom(new Vector2Int(gridX - 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX + 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX, gridY + 1));
            TryGenerateRoom(new Vector2Int(gridX, gridY - 1));
        }
        else if (roomCount < minRooms)
        {
            Debug.Log("Dungeon generate rooms fewer than the min limit. Regenerate the map");
            RegenerateRoom();
        }
        else if (!generationComplete)
        {
            foreach (var room in roomObjects)
            {
                Vector2Int index = room.GetComponent<Room>().RoomIndex;
                OpenDoor(room, index.x, index.y);
            }
            Debug.Log($"Generation Completed with {roomCount}");
            generationComplete = true;
            AssignStairRoom();
        }
    }

    private void StartRoomGenerationFromRoom(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        roomQueue.Enqueue(roomIndex);
        roomGrid[x, y] = 1;
        roomCount++;

        var initialRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        initialRoom.name = $"Room - {roomCount}";
        initialRoom.GetComponent<Room>().RoomIndex = roomIndex;
        roomObjects.Add(initialRoom);

        if (player != null)
        {
            Vector3 spawnPos = GetPositionFromGridIndex(roomIndex) + new Vector3(0, 0, 0);
            player.transform.position = spawnPos;
        }

        OpenDoor(initialRoom, x, y);

        //Vector3 spawnerPosition = GetPositionFromGridIndex(roomIndex) + new Vector3(0, 0, -1);
        //Instantiate(enemySpawnerPrefab, spawnerPosition, Quaternion.identity);
    }

    private bool TryGenerateRoom(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY)
            return false;

        if (roomCount >= maxRooms)
            return false;

        if (roomGrid[x,y] != 0)
            return false;

        if (Random.value < 0.5f && roomIndex != new Vector2Int(gridSizeX / 2, gridSizeY / 2))
            return false;

        if (CountAdjacentRooms(roomIndex) > 1)
            return false;

        roomGrid[x, y] = 1;
        roomCount++;

        GameObject newRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        if (newRoom == null)
        {
            Debug.LogError("Failed to instantiate room prefab.");
            return false;
        }

        Room roomScript = newRoom.GetComponent<Room>();
        newRoom.name = $"Room - {roomCount}";
        roomScript.RoomIndex = roomIndex;

        roomObjects.Add(newRoom);
        roomQueue.Enqueue(roomIndex);

        if (roomIndex != new Vector2Int(gridSizeX / 2, gridSizeY / 2))
        {
            Vector3 spawnerPosition = GetPositionFromGridIndex(roomIndex) + new Vector3(0, 0, -1);
            GameObject spawner = Instantiate(enemySpawnerPrefab, spawnerPosition, Quaternion.identity);

            MonsterSpawner mSpawner = spawner.GetComponent<MonsterSpawner>();
            if (mSpawner != null)
            {
                mSpawner.currentRoom = roomScript;
            }
            roomSpawners[roomIndex] = spawner;
        }

        return true;
    }

    private void RegenerateRoom()
    {
        foreach (var spawner in roomSpawners.Values)
        {
            if (spawner != null)
                Destroy(spawner);
        }

        roomObjects.ForEach(Destroy);
        roomObjects.Clear();
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue.Clear();
        roomCount = 0;
        generationComplete = false;
        roomSpawners.Clear();

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    private int CountAdjacentRooms(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;
        int count = 0;

        //Room neighbor check
        if (x > 0 && roomGrid[x - 1, y] != 0)
            count++;

        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0)
            count++;

        if (y > 0 && roomGrid[x, y - 1] != 0)
            count++;

        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0)
            count++;

        return count;
    }

    void OpenDoor(GameObject room, int x, int y)
    {
        if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY)
            return;

        Room newRoomObject = room.GetComponent<Room>();

        Room leftRoom = GetRoomAt(new Vector2Int(x - 1, y));
        Room rightRoom = GetRoomAt(new Vector2Int(x + 1, y));
        Room bottomRoom = GetRoomAt(new Vector2Int(x, y - 1));
        Room topRoom = GetRoomAt(new Vector2Int(x, y + 1));

        //if (x > 0 && roomGrid[x - 1, y] != 0)
        if (leftRoom != null)
        {
            newRoomObject.OpenDoor(Vector2Int.left);
            leftRoom.OpenDoor(Vector2Int.right);
        }

        //if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0)
        if (rightRoom != null)
        {
            newRoomObject.OpenDoor(Vector2Int.right);
            rightRoom.OpenDoor(Vector2Int.left);
        }

        //if (y > 0 && roomGrid[x, y - 1] != 0)
        if (bottomRoom != null)
        {
            newRoomObject.OpenDoor(Vector2Int.down);
            bottomRoom.OpenDoor(Vector2Int.up);
        }

        //if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0)
        if (topRoom != null)
        {
            newRoomObject.OpenDoor(Vector2Int.up);
            topRoom.OpenDoor(Vector2Int.down);
        }
    }

    void AssignStairRoom()
    {
        Vector2Int start = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        int maxDistance = -1;

        foreach (var roomObj in roomObjects)
        {
            Room room = roomObj.GetComponent<Room>();
            Vector2Int index = room.RoomIndex;

            int distance = Mathf.Abs(index.x - start.x) + Mathf.Abs(index.y - start.y);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestRoomIndex = index;
                stairRoomObject = roomObj;
            }
        }

        if (stairRoomObject != null)
        {
            Transform stair = stairRoomObject.transform.Find("ToNextLvl");
            if (stair != null)
            {
                stair.gameObject.SetActive(currentLevel < maxLevel);
                Debug.Log("Stair room assigned at: " + farthestRoomIndex);
            }

            Room stairRoom = stairRoomObject.GetComponent<Room>();
            if (stairRoom != null)
            {
                stairRoom.IsStairRoom = true;

                if (currentLevel == maxLevel)
                {
                    stairRoom.IsBossRoom = true;
                    Vector3 spawnPos = stairRoomObject.transform.position + new Vector3(0, 0, -1);
                    GameObject boss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);

                    EnemyHealth bossHealth = boss.GetComponent<EnemyHealth>();
                    if (bossHealth != null)
                    {
                        bossHealth.OnEnemyDied += OnBossDefeated;
                    }

                    //stairRoom.CloseAllDoors();
                }
            }

            if (roomSpawners.ContainsKey(farthestRoomIndex))
            {
                GameObject spawner = roomSpawners[farthestRoomIndex];
                if (spawner != null)
                {
                    Destroy(spawner);
                }
                roomSpawners.Remove(farthestRoomIndex);
            }
            else
            {
                MonsterSpawner spawner = stairRoomObject.GetComponentInChildren<MonsterSpawner>();
                if (spawner != null)
                {
                    Destroy(spawner.gameObject);
                }
            }
        }
    }

    private void OnBossDefeated(GameObject boss)
    {
        Debug.Log("Boss defeated!");
        bossDefeated = true;

        if (stairRoomObject != null)
        {
            Room stairRoom = stairRoomObject.GetComponent<Room>();
            stairRoom.OpenAllDoors();

            Vector3 treasurePosition = boss.transform.position;
            GameObject treasure = Instantiate(treasurePrefab, treasurePosition, Quaternion.identity);

            Treasure treasureScript = treasure.AddComponent<Treasure>();
            treasureScript.OnTreasureCollected += HandleTreasureCollected;
        }
    }
    private void HandleTreasureCollected()
    {
        Debug.Log("Treasure collected! Game completed!");
        GameOver.Instance.ShowGameOver(true);
    }

    public bool IsAtFinalLevel()
    {
        return currentLevel == maxLevel;
    }

    public bool IsBossDefeated()
    {
        return bossDefeated;
    }

    public void GoToNextLevel()
    {
        if (currentLevel < maxLevel)
        {
            currentLevel++;
            Debug.Log("Level Up! Now entering level: " + currentLevel);
            RegenerateRoom();
        }
        else
        {
            Debug.Log("You reached the final level!");
        }
    }

    Room GetRoomAt(Vector2Int index)
    {
        GameObject roomObject = roomObjects.Find(r => r.GetComponent<Room>().RoomIndex == index);
        if (roomObject != null)
            return roomObject.GetComponent<Room>();

        return null;
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitUntil(() => GameObject.FindGameObjectWithTag("Player") != null);
        player = GameObject.FindGameObjectWithTag("Player");

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;
        return new Vector3(roomWidth * (gridX - gridSizeX / 2), roomHeight * (gridY - gridSizeY / 2));
    }

    private void OnDrawGizmos()
    {
        Color gizmosColor = new Color(0, 1, 1, 0.5f);
        Gizmos.color = gizmosColor;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 position = GetPositionFromGridIndex(new Vector2Int(x, y));
                Gizmos.DrawWireCube(position, new Vector3(roomWidth, roomHeight, 1));
            }
        }
    }
}
