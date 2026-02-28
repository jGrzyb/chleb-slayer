using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.AI.Navigation;
using NavMeshPlus.Components;

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

                float noiseValue = Mathf.PerlinNoise(xCoord, yCoord);
                Vector3Int tilePosition = new Vector3Int(x + mapPositionX, y + mapPositionY, 0);

                groundTilemap.SetTile(tilePosition, groundTile);

                bool isEdge = (x == 0 || x == width - 1 || y == 0 || y == height - 1);
                if (isEdge || noiseValue <= 0.4f)
                {
                    obstacleTilemap.SetTile(tilePosition, obstacleTile);
                }
            }
        }
    }
}