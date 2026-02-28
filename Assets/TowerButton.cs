using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerButton : MonoBehaviour
{
    public TowerData towerData;

    public Button button;
    public TMP_Text woodText;
    public TMP_Text stoneText;
    public TMP_Text goldText;

    private static readonly Color affordable = Color.white;
    private static readonly Color lacking    = Color.red;

    void Awake()
    {
        button.onClick.AddListener(OnBuild);
    }

    void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (ResourceManager.instance == null || towerData == null) 
        {
            button.interactable = false;
            if (ResourceManager.instance == null) Debug.LogError("[TowerButton] ResourceManager not found in scene.");
            if (towerData == null)                Debug.LogError($"[TowerButton] TowerData not assigned on {gameObject.name}.");
            return;
        }

        ResourceManager rm = ResourceManager.instance;

        bool canWood  = rm.wood  >= towerData.woodCost;
        bool canStone = rm.stone >= towerData.stoneCost;
        bool canGold  = rm.gold  >= towerData.goldCost;

        woodText.text  = $"{towerData.woodCost}";
        stoneText.text = $"{towerData.stoneCost}";
        goldText.text  = $"{towerData.goldCost}";

        woodText.color  = canWood  ? affordable : lacking;
        stoneText.color = canStone ? affordable : lacking;
        goldText.color  = canGold  ? affordable : lacking;

        button.interactable = canWood && canStone && canGold;
    }

    void OnBuild()
    {
        ResourceManager.instance.BuyTower(towerData);
    }
}
