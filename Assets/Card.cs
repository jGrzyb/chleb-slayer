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

    public float bobAmplitude = 8f;
    public float bobSpeed = 1.5f;
    private Vector2 imageStartPos;
    private float bobOffset;

    public TMP_Text rarityName;
    public TMP_Text description;
    public TMP_Text cardName;
    

    
    void Start()
    {
        if (cardImage != null)
        {
            imageStartPos = cardImage.rectTransform.anchoredPosition;
            bobOffset = Random.Range(0f, Mathf.PI * 2f);
        }
    }

    void Update()
    {
        if (cardImage != null)
        {
            float y = Mathf.Sin(Time.time * bobSpeed + bobOffset) * bobAmplitude;
            cardImage.rectTransform.anchoredPosition = imageStartPos + new Vector2(0f, y);
        }
    }

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

        int rarityGradation = Random.Range(0, 9);
        if(rarityGradation < 3)
        {
            cardRarity = CardRarity.Common;
        }
        else if(rarityGradation < 6)
        {
            cardRarity = CardRarity.Rare;
        }
        else if(rarityGradation < 9)
        {
            cardRarity = CardRarity.Epic;
        }
        else
        {
            cardRarity = CardRarity.Legendary;
        }

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

        rarityName.text = cardRarity.ToString();

        int imageIndex = Random.Range(0, CardManager.instance.cardImages.Count);
        cardImage.sprite = CardManager.instance.cardImages[imageIndex];

        if(imageIndex<3)
        {
            cardName.text = CardManager.instance.femaleNames[Random.Range(0, CardManager.instance.femaleNames.Count)]+ " " + CardManager.instance.baking[imageIndex];
        }
        else if(imageIndex >= 3 && imageIndex <= 9)
        {
            cardName.text = CardManager.instance.maleNames[Random.Range(0, CardManager.instance.maleNames.Count)]+ " " + CardManager.instance.baking[imageIndex];
        }
        else
        {
            cardName.text = CardManager.instance.neutralNames[Random.Range(0, CardManager.instance.neutralNames.Count)]+ " " + CardManager.instance.baking[imageIndex];
        }
        description.text = "Dopust bozy pozwolil podniesc " + statsToBoost + " o " + valueToBoost + "%";
    }

 
    /// 1 Bulka ż
    /// 2. Tarta ż 
    /// 3. Mąka ż
    /// 
    /// 4. Chleb m 
    /// 5. Cruisant m
    /// 6. Donut m
    /// 7. Gofer m
    /// 8. Rogalik m 
    /// 9. Sernik m
    /// 
    /// 10. Ciastko n
    /// 11. Pieczywo n
    /// 
}

public enum CardRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

