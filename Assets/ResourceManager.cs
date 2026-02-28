using UnityEngine;
using System.Collections.Generic;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    public int wood  = 100;
    public int stone = 100;
    public int gold  = 100;
    public ResourceSprites resourceSprites;

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

    public bool BuyTower(TowerData tower)
    {
        if (!CanAfford(tower.woodCost, tower.stoneCost, tower.goldCost)) return false;
        Spend(tower.woodCost, tower.stoneCost, tower.goldCost);
        if (boughtTowers.ContainsKey(tower))
            boughtTowers[tower]++;
        else
            boughtTowers[tower] = 1;
        TowersBuilder.instance.RefreshEntry(tower);
        return true;
    }

    public int GetTowerCount(TowerData tower)
    {
        return boughtTowers.TryGetValue(tower, out int count) ? count : 0;
    }

    public void IncrementResource(Item.ItemType type)
    {
        switch (type)
            {
                case Item.ItemType.Wood:
                    wood++;
                    break;
                case Item.ItemType.Stone:
                    stone++;
                    break;
                case Item.ItemType.Gold:
                    gold++;
                    break;
                default:
                    Debug.LogWarning("Unknown item type collected.");
                    break;
            }
    }

    [Serializable]
    public class ResourceSprites
    {
        public Sprite woodSprite;
        public Sprite stoneSprite;
        public Sprite goldSprite;

        public Sprite GetSpriteForType(Item.ItemType type)
        {
            switch (type)
            {
                case Item.ItemType.Wood:
                    return woodSprite;
                case Item.ItemType.Stone:
                    return stoneSprite;
                case Item.ItemType.Gold:
                    return goldSprite;
                default:
                    Debug.LogWarning("Unknown item type: " + type);
                    return woodSprite;
            }
        }
    }
}
