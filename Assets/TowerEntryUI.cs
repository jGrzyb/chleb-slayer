using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerEntryUI : MonoBehaviour
{
    public Image towerImage;
    public TMP_Text towerNameText;
    public TMP_Text amountText;

    private TowerData data;

    public void Setup(TowerData towerData)
    {
        data = towerData;
        towerImage.sprite = towerData.towerSprite;
        towerNameText.text = towerData.towerName;
        UpdateAmount();
    }

    public void UpdateAmount()
    {
        int count = ResourceManager.instance.GetTowerCount(data);
        amountText.text = $"x{count}";
    }
}
