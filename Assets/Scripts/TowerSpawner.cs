using UnityEngine;
using UnityEngine.InputSystem;

public class TowerSpawner : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) TrySpawn(0);
        else if (Keyboard.current.digit2Key.wasPressedThisFrame) TrySpawn(1);
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) TrySpawn(2);
    }

    void TrySpawn(int index)
    {
        var allTowers = ResourceManager.instance.allTowers;
        var bought    = ResourceManager.instance.boughtTowers;

        if (index >= allTowers.Count) return;

        TowerData tower = allTowers[index];

        if (!bought.TryGetValue(tower, out int count) || count <= 0)
        {
            Debug.LogWarning($"Nie masz wieży {index + 1}!");
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPos.z = 0;

        Instantiate(tower.towerPrefab, worldPos, Quaternion.identity);

        bought[tower]--;
        TowersBuilder.instance.RefreshEntry(tower);
    }
}
