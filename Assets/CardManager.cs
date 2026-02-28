using UnityEngine;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{ 
    public List<Sprite> cardImages;
    public List<CardRarity> CardRarities = new List<CardRarity> { CardRarity.Common, CardRarity.Rare, CardRarity.Epic, CardRarity.Legendary };
    public List<string> StatsToBoost = new List<string>(); 

    public List<string> baking = new List<string>();
    public List<string> maleNames = new List<string>();
    public List<string> femaleNames = new List<string>(); 
    public List<string> neutralNames = new List<string>();

    public static CardManager instance;
    
    public void Awake()
    {
        instance = this;
    }

    
    
   
}
