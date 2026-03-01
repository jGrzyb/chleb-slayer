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
        foreach (Transform child in towerListParent)
            Destroy(child.gameObject);
        entries.Clear();

        List<TowerData> towers = ResourceManager.instance.allTowers;
        for (int i = 0; i < towers.Count; i++)
            SpawnEntry(towers[i], i);
    }

    void SpawnEntry(TowerData tower, int index)
    {
        GameObject go = Instantiate(towerEntryPrefab, towerListParent);
        TowerEntryUI entry = go.GetComponent<TowerEntryUI>();
        entry.Setup(tower, index);
        entries[tower] = entry;
    }

    public void RefreshEntry(TowerData tower)
    {
        if (entries.TryGetValue(tower, out TowerEntryUI entry))
            entry.UpdateAmount();
    }
}
