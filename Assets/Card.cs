using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour
{
    private CardRarity cardRarity;
    public string StatsToBoost { get; private set; }
    public int ValueToBoost { get; private set; }
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

    private static Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }

    private string TranslateStat(string stat) => stat.ToLower() switch
    {
        "damage"   => "obrażenia",
        "range"    => "dystans",
        "cooldown" => "szybkość",
        _          => stat
    };

    public void buildCard()
    {
        int statsIndex = Random.Range(0, CardManager.instance.StatsToBoost.Count);
        StatsToBoost = CardManager.instance.StatsToBoost[statsIndex];

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
                ValueToBoost = Random.Range(1, 10);
                this.GetComponent<Image>().color = HexColor("#538bac");
                break;
            case CardRarity.Rare:
                ValueToBoost = Random.Range(10, 20);
                this.GetComponent<Image>().color = HexColor("#5cad78");
                break;
            case CardRarity.Epic:
                ValueToBoost = Random.Range(20, 30);
                this.GetComponent<Image>().color = HexColor("#af41aa");
                break;  
            case CardRarity.Legendary:
                ValueToBoost = Random.Range(30, 40);
                this.GetComponent<Image>().color = HexColor("#bfa76b");
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
        description.text = "Dopust bozy pozwolil podniesc " + TranslateStat(StatsToBoost) + " o " + ValueToBoost + "%";
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

