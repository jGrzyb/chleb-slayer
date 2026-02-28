using UnityEngine;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    public int wood  = 100;
    public int stone = 100;
    public int gold  = 100;

    public List<TowerData> allTowers = new List<TowerData>();
    public Dictionary<TowerData, int> boughtTowers = new Dictionary<TowerData, int>();

    void Awake()
    {
        instance = this;
    }

    public bool CanAfford(int woodCost, int stoneCost, int goldCost)
    {
        return wood >= woodCost && stone >= stoneCost && gold >= goldCost;
    }

    public void Spend(int woodCost, int stoneCost, int goldCost)
    {
        wood  -= woodCost;
        stone -= stoneCost;
        gold  -= goldCost;
    }

    public void BuyTower(TowerData tower)
    {
        if (!CanAfford(tower.woodCost, tower.stoneCost, tower.goldCost)) return;
        Spend(tower.woodCost, tower.stoneCost, tower.goldCost);
        if (boughtTowers.ContainsKey(tower))
            boughtTowers[tower]++;
        else
            boughtTowers[tower] = 1;
        TowersBuilder.instance.RefreshEntry(tower);
    }

    public int GetTowerCount(TowerData tower)
    {
        return boughtTowers.TryGetValue(tower, out int count) ? count : 0;
    }
}
