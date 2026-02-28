using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour
{
    private CardRarity cardRarity;
    private string statsToBoost;
    private int valueToBoost;
    public Image cardImage;

    private static readonly Vector3 selectedScale = new Vector3(1.1f, 1.1f, 1f);
    private static readonly Vector3 normalScale = Vector3.one;

    public TMP_Text rarityName;
    public TMP_Text description;
    

    
    public void OnCardClicked()
    {
        UpgradeManager.instance.SelectCard(this);
    }

    public void SetSelected(bool selected)
    {
        transform.localScale = selected ? selectedScale : normalScale;
    }

    public void buildCard()
    {
        int statsIndex = Random.Range(0, CardManager.instance.StatsToBoost.Count);
        statsToBoost = CardManager.instance.StatsToBoost[statsIndex];

        int rarityIndex = Random.Range(0, CardManager.instance.CardRarities.Count);
        cardRarity = CardManager.instance.CardRarities[rarityIndex];

        switch (cardRarity) {
            case CardRarity.Common:
                valueToBoost = Random.Range(1, 10);
                this.GetComponent<Image>().color = Color.blue;
                break;
            case CardRarity.Rare:
                valueToBoost = Random.Range(10, 20);
                this.GetComponent<Image>().color = Color.green;
                break;
            case CardRarity.Epic:
                valueToBoost = Random.Range(20, 30);
                this.GetComponent<Image>().color = Color.magenta;
                break;  
            case CardRarity.Legendary:
                valueToBoost = Random.Range(30, 40);
                this.GetComponent<Image>().color = Color.yellow;
                break;
        }

        cardImage.sprite = CardManager.instance.cardImages[statsIndex];
        rarityName.text = cardRarity.ToString();
        description.text = "Dopust bozy pozwolil podniesc " + statsToBoost + " o " + valueToBoost + "%";
    }
}

public enum CardRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

