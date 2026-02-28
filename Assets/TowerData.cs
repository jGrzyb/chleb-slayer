using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Towers/TowerData")]
public class TowerData : ScriptableObject
{
    public string towerName;
    public Sprite towerSprite;
    public int woodCost;
    public int stoneCost;
    public int goldCost;
    public GameObject towerPrefab;
}
