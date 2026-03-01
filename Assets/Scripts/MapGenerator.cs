using NavMeshPlus.Components;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
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

    public void RegenerateEntireMap()
    {
        // 1. Czyszczenie starych kafelków
        groundTilemap.ClearAllTiles();
        obstacleTilemap.ClearAllTiles();

        // 3. Generowanie nowej mapy
        GenerateMap();

        // 4. Przebudowanie NavMesha dla nowej geometrii
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
    }
    private float cooldownTime = 2f;
    private float nextRegenTime = 0f;

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
        mapPositionX = -width/2;
        mapPositionY = -height/2;
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

        // Granice strefy bezpieczeñstwa
        int safeMinX = playerCell.x - playerSafeRadius;
        int safeMaxX = playerCell.x + playerSafeRadius;
        int safeMinY = playerCell.y - playerSafeRadius;
        int safeMaxY = playerCell.y + playerSafeRadius;

        int obstaclesPlaced = 0;
        int attempts = 0;
        // Podnosimy limit, bo przy du¿ym odstêpie ciê¿ej "trafiæ" w wolne miejsce
        int maxAttempts = numberOfObstacles * 100;

        while (obstaclesPlaced < numberOfObstacles && attempts < maxAttempts)
        {
            attempts++;

            int length = Random.Range(minLength, maxLength + 1);
            bool isHorizontal = Random.Range(0, 2) == 0;

            // Losujemy tak, by przeszkoda zmieœci³a siê wewn¹trz mapy (aby nie zast¹pi³a œcian).
            int localStartX = Random.Range(1, width - (isHorizontal ? length - 1 : 0) - 1);
            int localStartY = Random.Range(1, height - (isHorizontal ? 0 : length - 1) - 1);

            int startX = localStartX + mapPositionX;
            int startY = localStartY + mapPositionY;

            bool canPlace = true;

            // Ustalanie granic samej nowej przeszkody
            int minX = startX;
            int maxX = startX + (isHorizontal ? length - 1 : 0);
            int minY = startY;
            int maxY = startY + (isHorizontal ? 0 : length - 1);

            // Sprawdzenie dotyku prostopad³ego
        bool touchesWallPerpendicular = false;
        if (isHorizontal)
        {
            // Dotyka górnej lub dolnej œciany
            if (minY == mapPositionY || maxY == mapPositionY + height - 1)
                touchesWallPerpendicular = true;
        }
        else
        {
            // Dotyka lewej lub prawej œciany
            if (minX == mapPositionX || maxX == mapPositionX + width - 1)
                touchesWallPerpendicular = true;
        }

        //// Jeœli NIE dotyka prostopadle, sprawdŸ dystans od œciany
        //if (!touchesWallPerpendicular)
        //{
        //    int wallMinX = mapPositionX + wallSpacing;
        //    int wallMaxX = mapPositionX + width - 1 - wallSpacing;
        //    int wallMinY = mapPositionY + wallSpacing;
        //    int wallMaxY = mapPositionY + height - 1 - wallSpacing;

        //    if (isHorizontal)
        //    {
        //        if (minY < wallMinY || maxY > wallMaxY)
        //        {
        //            canPlace = false;
        //        }
        //    }
        //    else
        //    {
        //        if (minX < wallMinX || maxX > wallMaxX)
        //        {
        //            canPlace = false;
        //        }
        //    }
        //}

        if (!canPlace)
            continue;

            if (!(maxX < safeMinX || minX > safeMaxX || maxY < safeMinY || minY > safeMaxY))
            {
                // Przeszkoda wchodzi w strefê gracza, losujemy now¹ pozycjê
                continue;
            }

            // Skanowanie strefy buforowej od innych przeszkód
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
                        bool isTopOrBottomWall = (localCy <= 0 || localCy >= height - 1);
                        bool isOuterWall = isLeftOrRightWall || isTopOrBottomWall;

                        if (isOuterWall)
                        {
                            // Znaleziono œcianê w strefie buforowej - ca³kowicie j¹ ignorujemy
                            continue;
                        }
                        else
                        {
                            // To jest inna przeszkoda wygenerowana wczeœniej – odrzucamy pozycjê
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
            // Skanowanie strefy buforowej od przeszkód
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
                        bool isTopOrBottomWall = (localCy <= 0 || localCy >= height - 1);
                        bool isOuterWall = isLeftOrRightWall || isTopOrBottomWall;

                        if (isOuterWall)
                        {
                            bool isPerpendicularTouch = false;

                            if (isHorizontal)
                            {
                                // Sprawdzamy, czy przeszkoda fizycznie dotyka lewej lub prawej krawêdzi (odstêp równe 0)
                                bool touchesLeft = (minX == mapPositionX + 1);
                                bool touchesRight = (maxX == mapPositionX + width - 2);

                                // Jeœli skanujemy lew¹ œcianê i przeszkoda jej dotyka
                                if (localCx <= 0 && touchesLeft) isPerpendicularTouch = true;

                                // Jeœli skanujemy praw¹ œcianê i przeszkoda jej dotyka
                                if (localCx >= width - 1 && touchesRight) isPerpendicularTouch = true;
                            }
                            else
                            {
                                // Sprawdzamy, czy pionowa przeszkoda fizycznie dotyka dolnej lub górnej krawêdzi
                                bool touchesBottom = (minY == mapPositionY + 1);
                                bool touchesTop = (maxY == mapPositionY + height - 2);

                                if (localCy <= 0 && touchesBottom) isPerpendicularTouch = true;
                                if (localCy >= height - 1 && touchesTop) isPerpendicularTouch = true;
                            }

                            if (isPerpendicularTouch)
                            {
                                // Akceptujemy - przeszkoda jest prostopad³a i dotyka tej œciany "na styk"
                                continue;
                            }
                            else
                            {
                                // Odrzucamy - œciana jest równoleg³a LUB to œciana prostopad³a, ale jest w strefie buforowej (z odstêpem)
                                canPlace = false;
                                break;
                            }
                        }
                        else
                        {
                            // To inna postawiona wczeœniej przeszkoda – odrzucamy
                            canPlace = false;
                            break;
                        }
                    }
                }
            }
                if (canPlace)
            {
                for (int j = 0; j < length; j++)
                {
                    int currentX = startX + (isHorizontal ? j : 0);
                    int currentY = startY + (isHorizontal ? 0 : j);

                    obstacleTilemap.SetTile(new Vector3Int(currentX, currentY, 0), obstacleTile);
                }

                obstaclesPlaced++;
            }
        }

        if (obstaclesPlaced < numberOfObstacles)
        {
            Debug.LogWarning($"Wygenerowano {obstaclesPlaced}/{numberOfObstacles} przeszkód. Brakuje miejsca! Spróbuj zmniejszyæ wartoœæ 'Obstacle Spacing', zmniejszyæ liczbê przeszkód lub powiêkszyæ mapê.");
        }
    }
    void GenerateMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x + mapPositionX, y + mapPositionY, 0);

                groundTilemap.SetTile(tilePosition, groundTile);

                bool isEdge = (x == 0 || x == width - 1 || y == 0 || y == height - 1);
                if (isEdge)
                {
                    obstacleTilemap.SetTile(tilePosition, obstacleTile);
                }
            }
        }
        GenerateObstacles();
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
        int yMax = height - 1 - distanceFromWall;

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