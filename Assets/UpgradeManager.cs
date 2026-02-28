using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UpgradeManager : MonoBehaviour
{
    public List<GameObject> cardHolders;
    public List<Weapon> weapons;
    public GameObject cardPrefab;
    public GameObject weaponSelectorCurrent;
    public GameObject weaponSelectorUp;
    public GameObject weaponSelectorDown;
    public GameObject weaponSelectorParent;
    public Sprite defaultImage;
    public Sprite specialImage;

    public GameObject weaponSelectorPrefab;

    public static UpgradeManager instance;

    private int currentWeaponIndex = 0;
    private Card selectedCard = null;

    public Card SelectedCard => selectedCard;
    public Weapon ActiveWeapon => (weapons != null && weapons.Count > 0) ? weapons[currentWeaponIndex] : null;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GenerateCard();
        GenerateWeaponCounter();
        UpdateWeaponSelector();
    }

    void Update()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll > 0f) ScrollUp();
        else if (scroll < 0f) ScrollDown();
    }

    public void SelectCard(Card card)
    {
        if (selectedCard != null) selectedCard.SetSelected(false);
        selectedCard = card;
        selectedCard.SetSelected(true);
    }

    public void DeselectCard()
    {
        if (selectedCard != null)
        {
            selectedCard.SetSelected(false);
            selectedCard = null;
        }
    }

    public void ScrollUp()
    {
        if (weapons.Count == 0 || currentWeaponIndex == 0) return;
        DeselectCard();
        currentWeaponIndex--;
        UpdateWeaponSelector();
    }

    public void ScrollDown()
    {
        if (weapons.Count == 0 || currentWeaponIndex == weapons.Count - 1) return;
        DeselectCard();
        currentWeaponIndex++;
        UpdateWeaponSelector();
    }

    void UpdateWeaponSelector()
    {
        if (weapons.Count == 0) return;

        int last = weapons.Count - 1;

        SetSlotSprite(weaponSelectorCurrent, GetSprite(currentWeaponIndex));

        if (currentWeaponIndex == 0)
            SetSlotSprite(weaponSelectorUp, null);
        else
            SetSlotSprite(weaponSelectorUp, GetSprite(currentWeaponIndex - 1));

        if (currentWeaponIndex == last)
            SetSlotSprite(weaponSelectorDown, null);
        else
            SetSlotSprite(weaponSelectorDown, GetSprite(currentWeaponIndex + 1));

        if (weaponSelectorParent == null) return;
        for (int i = 0; i < weaponSelectorParent.transform.childCount; i++)
        {
            Transform child = weaponSelectorParent.transform.GetChild(i);
            SetSlotSprite(child.gameObject, i == currentWeaponIndex ? specialImage : defaultImage);
        }
    }

    Sprite GetSprite(int index)
    {
        if (index < 0 || index >= weapons.Count) return null;
        return weapons[index]?.weaponSprite;
    }

    void SetSlotSprite(GameObject slot, Sprite sprite)
    {
        if (slot == null) return;
        Image img = slot.GetComponent<Image>();
        if (img == null) return;
        img.sprite = sprite;
        img.enabled = sprite != null;
    }

    void GenerateCard()
    {
        foreach (var cardHolder in cardHolders)
        {
            GameObject card = Instantiate(cardPrefab, cardHolder.transform);
            RectTransform rt = card.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            Card cardComponent = card.GetComponent<Card>();
            cardComponent.buildCard();
            Button btn = card.GetComponent<Button>();
            if (btn != null) btn.onClick.AddListener(cardComponent.OnCardClicked);
        }
    }

    void GenerateWeaponCounter()
    {
        foreach (var weapon in weapons)
            Instantiate(weaponSelectorPrefab, weaponSelectorParent.transform);
    }
}
