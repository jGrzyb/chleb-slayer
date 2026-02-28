using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerEntryUI : MonoBehaviour
{
    public Image towerImage;
    public TMP_Text indexText;
    public TMP_Text amountText;

    private TowerData data;

    public void Setup(TowerData towerData, int index)
    {
        data = towerData;
        if (towerImage != null) towerImage.sprite = towerData.towerSprite;
        if (indexText != null)  indexText.text = (index + 1).ToString();
        UpdateAmount();
    }

    public void UpdateAmount()
    {
        int count = ResourceManager.instance.GetTowerCount(data);
        if (amountText != null) amountText.text = $"{count}";
    }
}
