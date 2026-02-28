using UnityEngine;
using System.Collections.Generic;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;
    public event Action<int> OnWoodChanged = delegate { };
    public event Action<int> OnStoneChanged = delegate { };
    public event Action<int> OnGoldChanged = delegate { };

    private int _wood  = 100;
    private int _stone = 100;
    private int _gold  = 100;

    public int wood {
        get => _wood;
        set {
            _wood = value;
            OnWoodChanged.Invoke(_wood);
        }
    }
    
    public int stone {
        get => _stone;
        set {
            _stone = value;
            OnStoneChanged.Invoke(_stone);
        }
    }

    public int gold {
        get => _gold;
        set {
            _gold = value;
            OnGoldChanged.Invoke(_gold);
        }
    }

    public ResourceSprites resourceSprites;

    public List<TowerData> allTowers = new List<TowerData>();
    public Dictionary<TowerData, int> boughtTowers = new Dictionary<TowerData, int>();

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
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
                    GameManager.I.endStats.woodCollected ++;
                break;
                case Item.ItemType.Stone:
                    stone++;
                    GameManager.I.endStats.stoneCollected++;
                break;
                case Item.ItemType.Gold:
                    gold++;
                    GameManager.I.endStats.goldCollected++;
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
