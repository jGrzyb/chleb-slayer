using NavMeshPlus.Components;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Cinemachine;
public class MapGenerator : MonoBehaviour
{
    [Header("Ustawienia Mapy")]
    public int width = 50;
    public int height = 50;
    public float noiseScale = 5f;

    [Header("Pozycja Mapy w Œwiecie")]
    public int mapPositionX = -25;
    public int mapPositionY = -25;

    [Header("Warstwy (Tilemapy)")]
    public Tilemap groundTilemap;
    public Tilemap obstacleTilemap;

    [Header("Kafelki")]
    public TileBase groundTile;
    public TileBase obstacleTile;

    [Header("Przeciwnicy")]
    public GameObject enemySpawnerPrefab;
    [Tooltip("Odleg³oœæ spawnera od œciany w kafelkach. 1 oznacza kratkê tu¿ obok œciany.")]
    public int distanceFromWall = 1;

    private NavMeshPlus.Components.NavMeshSurface navMeshSurface;

    private float offsetX;
    private float offsetY;
    private int[,] mapData;

    void Start()
    {
        navMeshSurface = FindAnyObjectByType<NavMeshPlus.Components.NavMeshSurface>();
        offsetX = Random.Range(-10000f, 10000f);
        offsetY = Random.Range(-10000f, 10000f);
        GenerateMap();
        SpawnEnemySpawners();
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
        GenerateCameraBounds();
    }

    void GenerateMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * noiseScale + offsetX;
                float yCoord = (float)y / height * noiseScale + offsetY;

                //float noiseValue = Mathf.PerlinNoise(xCoord, yCoord);
                Vector3Int tilePosition = new Vector3Int(x + mapPositionX, y + mapPositionY, 0);

                groundTilemap.SetTile(tilePosition, groundTile);

                bool isEdge = (x == 0 || x == width - 1 || y == 0 || y == height - 1);
                if (isEdge)
                {
                    obstacleTilemap.SetTile(tilePosition, obstacleTile);
                }
            }
        }
    }
    void SpawnEnemySpawners()
    {
        if (enemySpawnerPrefab == null)
        {
            Debug.LogWarning("Nie przypisano prefabu spawnera w inspektorze!");
            return;
        }

        // Zabezpieczenie na wypadek zbyt ma³ej mapy lub zbyt du¿ej odleg³oœci od œciany
        if (distanceFromWall * 2 >= width || distanceFromWall * 2 >= height)
        {
            Debug.LogError("Mapa jest za ma³a, aby umieœciæ spawnery w takiej odleg³oœci od œciany!");
            return;
        }

        // Obliczamy lokalne wspó³rzêdne wewnêtrznych k¹tów
        int xMin = distanceFromWall;
        int xMax = width - 1 - distanceFromWall;
        int yMin = distanceFromWall;
        int yMax = height - 1 - distanceFromWall;

        // Tworzymy listê 4 docelowych miejsc na siatce (przesuniêtych o pozycjê mapy)
        Vector3Int[] cornerPositions = new Vector3Int[]
        {
            new Vector3Int(xMin + mapPositionX, yMin + mapPositionY, 0), // Lewy dolny
            new Vector3Int(xMax + mapPositionX, yMin + mapPositionY, 0), // Prawy dolny
            new Vector3Int(xMin + mapPositionX, yMax + mapPositionY, 0), // Lewy górny
            new Vector3Int(xMax + mapPositionX, yMax + mapPositionY, 0)  // Prawy górny
        };

        // Generujemy prefab we wszystkich 4 pozycjach
        foreach (Vector3Int gridPos in cornerPositions)
        {
            // U¿ywamy GetCellCenterWorld, aby pobraæ dok³adny œrodek kafelka na scenie 
            Vector3 worldPos = groundTilemap.GetCellCenterWorld(gridPos);

            // Tworzymy spawner jako dziecko MapGeneratora (this.transform), co pomaga w utrzymaniu porz¹dku
            Instantiate(enemySpawnerPrefab, worldPos, Quaternion.identity, this.transform);
        }
    }
    // MAGIA: Algorytm wyszukuj¹cy odciête pokoje
    void RemoveDisconnectedRegions()
    {
        int[,] visited = new int[width, height];
        List<List<Vector2Int>> allWalkableRegions = new List<List<Vector2Int>>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapData[x, y] == 0 && visited[x, y] == 0)
                {
                    List<Vector2Int> newRegion = new List<Vector2Int>();
                    Queue<Vector2Int> queue = new Queue<Vector2Int>();

                    queue.Enqueue(new Vector2Int(x, y));
                    visited[x, y] = 1;

                    while (queue.Count > 0)
                    {
                        Vector2Int tile = queue.Dequeue();
                        newRegion.Add(tile);

                        Vector2Int[] neighbors = {
                            new Vector2Int(tile.x, tile.y + 1),
                            new Vector2Int(tile.x, tile.y - 1),
                            new Vector2Int(tile.x - 1, tile.y),
                            new Vector2Int(tile.x + 1, tile.y)
                        };

                        foreach (Vector2Int n in neighbors)
                        {
                            if (n.x >= 0 && n.x < width && n.y >= 0 && n.y < height)
                            {
                                if (mapData[n.x, n.y] == 0 && visited[n.x, n.y] == 0)
                                {
                                    visited[n.x, n.y] = 1;
                                    queue.Enqueue(n);
                                }
                            }
                        }
                    }
                    allWalkableRegions.Add(newRegion);
                }
            }
        }

        if (allWalkableRegions.Count == 0) return;

        int largestRegionIndex = 0;
        for (int i = 1; i < allWalkableRegions.Count; i++)
        {
            if (allWalkableRegions[i].Count > allWalkableRegions[largestRegionIndex].Count)
            {
                largestRegionIndex = i;
            }
        }

        for (int i = 0; i < allWalkableRegions.Count; i++)
        {
            if (i != largestRegionIndex)
            {
                foreach (Vector2Int tile in allWalkableRegions[i])
                {
                    mapData[tile.x, tile.y] = 1;
                }
            }
        }
    }

    void GenerateCameraBounds()
    {
        // 1. Dodajemy collider bezpoœrednio do obiektu Tilemapy!
        // Dziêki temu mamy pewnoœæ, ¿e jego pozycje idealnie pokrywaj¹ siê z siatk¹ kafelków.
        PolygonCollider2D boundsCollider = groundTilemap.gameObject.GetComponent<PolygonCollider2D>();
        if (boundsCollider == null)
        {
            boundsCollider = groundTilemap.gameObject.AddComponent<PolygonCollider2D>();
        }

        boundsCollider.isTrigger = true;

        // 2. Obliczamy 4 rogi mapy (wspó³rzêdne lokalne Tilemapy)
        Vector2[] corners = new Vector2[4];
        corners[0] = new Vector2(mapPositionX, mapPositionY);
        corners[1] = new Vector2(mapPositionX + width, mapPositionY);
        corners[2] = new Vector2(mapPositionX + width, mapPositionY + height);
        corners[3] = new Vector2(mapPositionX, mapPositionY + height);

        boundsCollider.points = corners;

        // 3. Skrypt SAM szuka wirtualnej kamery z Confinerem na scenie
        Unity.Cinemachine.CinemachineConfiner2D foundConfiner = FindAnyObjectByType<Unity.Cinemachine.CinemachineConfiner2D>();

        if (foundConfiner != null)
        {
            // Przypisujemy wygenerowany przed chwil¹ collider
            foundConfiner.BoundingShape2D = boundsCollider;
            // Odœwie¿amy pamiêæ kamery
            foundConfiner.InvalidateBoundingShapeCache();

            Debug.Log("<color=green>Sukces!</color> Confiner znalaz³ mapê o rozmiarze " + width + "x" + height);
        }
        else
        {
            Debug.LogError("<color=red>B³¹d:</color> Nie znaleziono komponentu CinemachineConfiner2D na scenie!");
        }
    }

}