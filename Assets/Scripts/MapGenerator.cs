using NavMeshPlus.Components;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[System.Serializable]
public class ObstaclePattern
{
    [Tooltip("Przeszkody")]
    public TileBase[] tiles;
}

public class MapGenerator : MonoBehaviour
{
    [Header("Ustawienia Mapy")]
    public int width = 50;
    public int height = 50;

    [Header("Ustawienia przeszkód")]
    public int numberOfObstacles = 10;
    public int minLength = 2;
    public int maxLength = 5;
    public int obstacleSpacing = 2;
    public int wallSpacing = 2;

    private int mapPositionX;
    private int mapPositionY;

    [Tooltip("Szansa (w %) na pojawienie siê dekoracji na pustym polu")]
    [Range(0f, 100f)]
    public float decorationDensity = 15f;

    [Header("Warstwy (Tilemapy)")]
    public Tilemap groundTilemap;
    public Tilemap obstacleTilemap;
    public Tilemap decorationsTilemap;

    [Header("Pod³oga")]
    public TileBase groundTile;
    public TileBase secondGroundTile;

    [Header("Œciany - Krawêdzie (Górny rz¹d)")]
    public TileBase[] wallTopTiles;
    public TileBase wallBottom;
    public TileBase wallLeft;
    public TileBase wallRight;

    // --- NOWE: Dolny rz¹d górnej œciany ---
    [Header("Œciany - Górna (Dolny rz¹d)")]
    [Tooltip("Te kafelki pojawi¹ siê bezpoœrednio pod górn¹ krawêdzi¹ œciany")]
    public TileBase[] wallTopLowerTiles;
    public TileBase cornerTopLeftLower;
    public TileBase cornerTopRightLower;
    // --------------------------------------

    [Header("Œciany - Rogi")]
    public TileBase cornerTopLeft;
    public TileBase cornerTopRight;
    public TileBase cornerBottomLeft;
    public TileBase cornerBottomRight;

    [Header("Dekoracje na mapie")]
    [Tooltip("Kafelki u¿ywane jako dekoracje (trawa, kamyki, itp.)")]
    public TileBase[] decorationTiles;

    [Header("Gotowe szablony przeszkód")]
    public ObstaclePattern[] singleObstacles;
    public ObstaclePattern[] horizontalObstacles;
    public ObstaclePattern[] verticalObstacles;

    [Header("Przeciwnicy")]
    public GameObject enemySpawnerPrefab;
    [Tooltip("Odleg³oœæ spawnera od œciany w kafelkach. 1 oznacza kratkê tu¿ obok œciany.")]
    public int distanceFromWall = 1;

    private NavMeshPlus.Components.NavMeshSurface navMeshSurface;
    private float cooldownTime = 2f;
    private float nextRegenTime = 0f;

    public void RegenerateEntireMap()
    {
        groundTilemap.ClearAllTiles();
        obstacleTilemap.ClearAllTiles();
        decorationsTilemap.ClearAllTiles();

        GenerateMap();

        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
    }

    void Update()
    {
        if (Time.time >= nextRegenTime && Keyboard.current != null && Keyboard.current.shiftKey.wasPressedThisFrame)
        {
            RegenerateEntireMap();
            nextRegenTime = Time.time + cooldownTime;
        }
    }

    void Awake()
    {
        groundTilemap.ClearAllTiles();
        obstacleTilemap.ClearAllTiles();
        decorationsTilemap.ClearAllTiles();
        mapPositionX = -width / 2;
        mapPositionY = -height / 2;
        GenerateMap();
        SpawnEnemySpawners();
    }

    private void Start()
    {
        navMeshSurface = FindAnyObjectByType<NavMeshPlus.Components.NavMeshSurface>();
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
        GenerateCameraBounds();
    }

    public void GenerateObstacles()
    {
        int playerSafeRadius = 3;
        Vector3Int playerCell = Vector3Int.zero;

        Player playerObj = FindAnyObjectByType<Player>();
        if (playerObj != null)
        {
            playerCell = groundTilemap.WorldToCell(playerObj.transform.position);
        }

        int safeMinX = playerCell.x - playerSafeRadius;
        int safeMaxX = playerCell.x + playerSafeRadius;
        int safeMinY = playerCell.y - playerSafeRadius;
        int safeMaxY = playerCell.y + playerSafeRadius;

        int obstaclesPlaced = 0;
        int attempts = 0;
        int maxAttempts = numberOfObstacles * 100;

        while (obstaclesPlaced < numberOfObstacles && attempts < maxAttempts)
        {
            attempts++;

            bool isHorizontal = Random.Range(0, 2) == 0;
            int targetLength = Random.Range(minLength, maxLength + 1);

            List<ObstaclePattern> validPool = new List<ObstaclePattern>();
            validPool.AddRange(singleObstacles);
            if (isHorizontal) validPool.AddRange(horizontalObstacles);
            else validPool.AddRange(verticalObstacles);

            if (validPool.Count == 0) continue;

            List<ObstaclePattern> chosenSequence = new List<ObstaclePattern>();
            int currentLength = 0;

            while (currentLength < targetLength)
            {
                int remainingSpace = targetLength - currentLength;
                List<ObstaclePattern> fittingPatterns = new List<ObstaclePattern>();

                foreach (ObstaclePattern p in validPool)
                {
                    if (p.tiles != null && p.tiles.Length > 0 && p.tiles.Length <= remainingSpace)
                    {
                        fittingPatterns.Add(p);
                    }
                }

                if (fittingPatterns.Count == 0) break;

                ObstaclePattern pickedPattern = fittingPatterns[Random.Range(0, fittingPatterns.Count)];
                chosenSequence.Add(pickedPattern);
                currentLength += pickedPattern.tiles.Length;
            }

            int length = currentLength;
            if (length == 0) continue;

            // --- ZMIANA: Górna granica spawnowania przeszkód obni¿ona o 1 (-2 zamiast -1) ---
            int localStartX = Random.Range(1, width - (isHorizontal ? length - 1 : 0) - 1);
            int localStartY = Random.Range(1, height - (isHorizontal ? 0 : length - 1) - 2);

            int startX = localStartX + mapPositionX;
            int startY = localStartY + mapPositionY;

            bool canPlace = true;

            int minX = startX;
            int maxX = startX + (isHorizontal ? length - 1 : 0);
            int minY = startY;
            int maxY = startY + (isHorizontal ? 0 : length - 1);

            bool touchesWallPerpendicular = false;
            if (isHorizontal)
            {
                // --- ZMIANA: Górna œciana zaczyna siê od mapPositionY + height - 2 ---
                if (minY == mapPositionY || maxY == mapPositionY + height - 2)
                    touchesWallPerpendicular = true;
            }
            else
            {
                if (minX == mapPositionX || maxX == mapPositionX + width - 1)
                    touchesWallPerpendicular = true;
            }

            if (!(maxX < safeMinX || minX > safeMaxX || maxY < safeMinY || minY > safeMaxY))
            {
                continue;
            }

            int checkMinX = minX - obstacleSpacing;
            int checkMaxX = maxX + obstacleSpacing;
            int checkMinY = minY - obstacleSpacing;
            int checkMaxY = maxY + obstacleSpacing;

            for (int cx = checkMinX; cx <= checkMaxX && canPlace; cx++)
            {
                for (int cy = checkMinY; cy <= checkMaxY; cy++)
                {
                    Vector3Int pos = new Vector3Int(cx, cy, 0);

                    if (obstacleTilemap.HasTile(pos))
                    {
                        int localCx = cx - mapPositionX;
                        int localCy = cy - mapPositionY;

                        bool isLeftOrRightWall = (localCx <= 0 || localCx >= width - 1);
                        // --- ZMIANA: Górna œciana zaczyna siê od wysokoœci height - 2 ---
                        bool isTopOrBottomWall = (localCy <= 0 || localCy >= height - 2);

                        if (isLeftOrRightWall || isTopOrBottomWall) continue;
                        else
                        {
                            canPlace = false;
                            break;
                        }
                    }
                }
            }

            checkMinX = minX - wallSpacing;
            checkMaxX = maxX + wallSpacing;
            checkMinY = minY - wallSpacing;
            checkMaxY = maxY + wallSpacing;

            for (int cx = checkMinX; cx <= checkMaxX && canPlace; cx++)
            {
                for (int cy = checkMinY; cy <= checkMaxY; cy++)
                {
                    Vector3Int pos = new Vector3Int(cx, cy, 0);

                    if (obstacleTilemap.HasTile(pos))
                    {
                        int localCx = cx - mapPositionX;
                        int localCy = cy - mapPositionY;

                        bool isLeftOrRightWall = (localCx <= 0 || localCx >= width - 1);
                        // --- ZMIANA: Górna œciana zajmuje height - 1 oraz height - 2 ---
                        bool isTopOrBottomWall = (localCy <= 0 || localCy >= height - 2);
                        bool isOuterWall = isLeftOrRightWall || isTopOrBottomWall;

                        if (isOuterWall)
                        {
                            bool isPerpendicularTouch = false;
                            if (isHorizontal)
                            {
                                bool touchesLeft = (minX == mapPositionX + 1);
                                bool touchesRight = (maxX == mapPositionX + width - 2);
                                if (localCx <= 0 && touchesLeft) isPerpendicularTouch = true;
                                if (localCx >= width - 1 && touchesRight) isPerpendicularTouch = true;
                            }
                            else
                            {
                                bool touchesBottom = (minY == mapPositionY + 1);
                                // --- ZMIANA: Wewnêtrzna granica dla górnej œciany to teraz height - 3 ---
                                bool touchesTop = (maxY == mapPositionY + height - 3);
                                if (localCy <= 0 && touchesBottom) isPerpendicularTouch = true;
                                if (localCy >= height - 2 && touchesTop) isPerpendicularTouch = true;
                            }

                            if (isPerpendicularTouch) continue;
                            else
                            {
                                canPlace = false;
                                break;
                            }
                        }
                        else
                        {
                            canPlace = false;
                            break;
                        }
                    }
                }
            }

            if (canPlace)
            {
                int currentOffset = 0;

                foreach (ObstaclePattern pattern in chosenSequence)
                {
                    for (int j = 0; j < pattern.tiles.Length; j++)
                    {
                        int currentX = startX + (isHorizontal ? currentOffset + j : 0);
                        int currentY = startY + (isHorizontal ? 0 : currentOffset + j);

                        TileBase tileToPlace = pattern.tiles[j];
                        obstacleTilemap.SetTile(new Vector3Int(currentX, currentY, 0), tileToPlace);
                    }
                    currentOffset += pattern.tiles.Length;
                }
                obstaclesPlaced++;
            }
        }

        if (obstaclesPlaced < numberOfObstacles)
        {
            Debug.LogWarning($"Wygenerowano {obstaclesPlaced}/{numberOfObstacles} przeszkód. Brakuje miejsca!");
        }
    }

    void GenerateMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x + mapPositionX, y + mapPositionY, 0);

                TileBase currentGroundTile = ((x + y) % 2 == 0) ? groundTile : secondGroundTile;
                groundTilemap.SetTile(tilePosition, currentGroundTile);

                bool isEdge = (x == 0 || x == width - 1 || y == 0 || y == height - 1);
                bool isLeft = (x == 0);
                bool isRight = (x == width - 1);
                bool isBottom = (y == 0);
                bool isTop = (y == height - 1);

                if (isLeft || isRight || isBottom || isTop)
                {
                    TileBase wallTileToPlace = null;

                    if (isTop && isLeft)
                    {
                        wallTileToPlace = cornerTopLeft;
                        // --- NOWE: Stawiamy dolny róg ---
                        if (cornerTopLeftLower != null)
                            obstacleTilemap.SetTile(tilePosition + Vector3Int.down, cornerTopLeftLower);
                    }
                    else if (isTop && isRight)
                    {
                        wallTileToPlace = cornerTopRight;
                        // --- NOWE: Stawiamy dolny róg ---
                        if (cornerTopRightLower != null)
                            obstacleTilemap.SetTile(tilePosition + Vector3Int.down, cornerTopRightLower);
                    }
                    else if (isBottom && isLeft)
                    {
                        wallTileToPlace = cornerBottomLeft;
                    }
                    else if (isBottom && isRight)
                    {
                        wallTileToPlace = cornerBottomRight;
                    }
                    else if (isTop)
                    {
                        if (wallTopTiles != null && wallTopTiles.Length > 0)
                        {
                            int index = (x - 1) % wallTopTiles.Length;
                            wallTileToPlace = wallTopTiles[index];

                            // --- NOWE: Stawiamy doln¹ czêœæ górnej œciany ---
                            if (wallTopLowerTiles != null && wallTopLowerTiles.Length > 0)
                            {
                                int lowerIndex = (x - 1) % wallTopLowerTiles.Length;
                                obstacleTilemap.SetTile(tilePosition + Vector3Int.down, wallTopLowerTiles[lowerIndex]);
                            }
                        }
                    }
                    else if (isBottom)
                    {
                        wallTileToPlace = wallBottom;
                    }
                    else if (isLeft)
                    {
                        wallTileToPlace = wallLeft;
                    }
                    else if (isRight)
                    {
                        wallTileToPlace = wallRight;
                    }

                    if (wallTileToPlace != null)
                    {
                        obstacleTilemap.SetTile(tilePosition, wallTileToPlace);
                    }
                }
            }
        }
        GenerateObstacles();
        GenerateDecorations();
    }

    void SpawnEnemySpawners()
    {
        if (enemySpawnerPrefab == null)
        {
            Debug.LogWarning("Nie przypisano prefabu spawnera w inspektorze!");
            return;
        }

        if (distanceFromWall * 2 >= width || distanceFromWall * 2 >= height)
        {
            Debug.LogError("Mapa jest za ma³a, aby umieœciæ spawnery w takiej odleg³oœci od œciany!");
            return;
        }

        int xMin = distanceFromWall;
        int xMax = width - 1 - distanceFromWall;
        int yMin = distanceFromWall;

        // --- ZMIANA: Górna granica uwzglêdnia grubsz¹ o 1 kratkê œcianê (-2 zamiast -1) ---
        int yMax = height - 2 - distanceFromWall;

        Vector3Int[] cornerPositions = new Vector3Int[]
        {
            new Vector3Int(xMin + mapPositionX, yMin + mapPositionY, 0),
            new Vector3Int(xMax + mapPositionX, yMin + mapPositionY, 0),
            new Vector3Int(xMin + mapPositionX, yMax + mapPositionY, 0),
            new Vector3Int(xMax + mapPositionX, yMax + mapPositionY, 0)
        };

        foreach (Vector3Int gridPos in cornerPositions)
        {
            Vector3 worldPos = groundTilemap.GetCellCenterWorld(gridPos);
            Instantiate(enemySpawnerPrefab, worldPos, Quaternion.identity, this.transform);
        }
    }

    void GenerateDecorations()
    {
        if (decorationTiles == null || decorationTiles.Length == 0) return;

        for (int x = 1; x < width - 1; x++)
        {
            // --- ZMIANA: Dekoracje omijaj¹ doln¹ czêœæ górnej œciany (height - 2) ---
            for (int y = 1; y < height - 2; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x + mapPositionX, y + mapPositionY, 0);

                if (!obstacleTilemap.HasTile(tilePosition))
                {
                    if (Random.Range(0f, 100f) <= decorationDensity)
                    {
                        TileBase randomDeco = decorationTiles[Random.Range(0, decorationTiles.Length)];
                        decorationsTilemap.SetTile(tilePosition, randomDeco);
                    }
                }
            }
        }
    }

    void GenerateCameraBounds()
    {
        PolygonCollider2D boundsCollider = groundTilemap.gameObject.GetComponent<PolygonCollider2D>();
        if (boundsCollider == null)
        {
            boundsCollider = groundTilemap.gameObject.AddComponent<PolygonCollider2D>();
        }

        boundsCollider.isTrigger = true;

        Vector2[] corners = new Vector2[4];
        corners[0] = new Vector2(mapPositionX, mapPositionY);
        corners[1] = new Vector2(mapPositionX + width, mapPositionY);
        corners[2] = new Vector2(mapPositionX + width, mapPositionY + height);
        corners[3] = new Vector2(mapPositionX, mapPositionY + height);

        boundsCollider.points = corners;

        Unity.Cinemachine.CinemachineConfiner2D foundConfiner = FindAnyObjectByType<Unity.Cinemachine.CinemachineConfiner2D>();

        if (foundConfiner != null)
        {
            foundConfiner.BoundingShape2D = boundsCollider;
            foundConfiner.InvalidateBoundingShapeCache();
            Debug.Log("<color=green>Sukces!</color> Confiner znalaz³ mapê o rozmiarze " + width + "x" + height);
        }
        else
        {
            Debug.LogError("<color=red>B³¹d:</color> Nie znaleziono komponentu CinemachineConfiner2D na scenie!");
        }
    }
}