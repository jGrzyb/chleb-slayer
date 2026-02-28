using UnityEngine;
using UnityEngine.InputSystem;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject towerPrefab;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            SpawnTowerAtMouse();
        }
    }

    void SpawnTowerAtMouse()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPos.z = 0;
        Instantiate(towerPrefab, worldPos, Quaternion.identity);
    }
}