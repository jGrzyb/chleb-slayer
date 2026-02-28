using UnityEngine;
using System.Collections.Generic;

public class TowersBuilder : MonoBehaviour
{
    public static TowersBuilder instance;

    public GameObject towerEntryPrefab;
    public Transform towerListParent;

    private Dictionary<TowerData, TowerEntryUI> entries = new Dictionary<TowerData, TowerEntryUI>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        foreach (TowerData tower in ResourceManager.instance.allTowers)
            SpawnEntry(tower);
    }

    void SpawnEntry(TowerData tower)
    {
        GameObject go = Instantiate(towerEntryPrefab, towerListParent);
        TowerEntryUI entry = go.GetComponent<TowerEntryUI>();
        entry.Setup(tower);
        entries[tower] = entry;
    }

    public void RefreshEntry(TowerData tower)
    {
        if (entries.TryGetValue(tower, out TowerEntryUI entry))
            entry.UpdateAmount();
    }
}
