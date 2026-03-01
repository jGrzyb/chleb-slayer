using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class bakedNextScene : MonoBehaviour
{
    public string sceneName;
    public Button nextButton;

    void Update()
    {
        if (nextButton != null)
            nextButton.interactable = UpgradeManager.instance?.SelectedCard != null;
    }

    public void NextScene()
    {
        Card card = UpgradeManager.instance?.SelectedCard;
        if (card == null)
        {
            Debug.LogWarning("[bakedNextScene] Brak aktywnej karty — wybierz kartę przed przejściem!");
            return;
        }

        Weapon weapon = UpgradeManager.instance.ActiveWeapon;
        if (weapon == null)
        {
            Debug.LogWarning("[bakedNextScene] Brak aktywnej broni!");
            return;
        }

        weapon.ApplyBoost(card.StatsToBoost, card.ValueToBoost);
        Debug.Log($"[bakedNextScene] Zaaplikowano boost '{card.StatsToBoost}' +{card.ValueToBoost}% na broń '{weapon.weaponName}'");

        FadeManager.I.LoadSceneWithFade(sceneName);
    }
}
