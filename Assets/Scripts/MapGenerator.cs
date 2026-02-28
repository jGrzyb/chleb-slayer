using NavMeshPlus.Components;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
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
}