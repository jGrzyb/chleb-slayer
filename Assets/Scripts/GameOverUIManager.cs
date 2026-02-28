using UnityEngine;
using TMPro;

public class GameOverUIManager : MonoBehaviour
{
    [Header("G³ówne Statystyki")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI towersBuiltText;

    [Header("Zabójstwa")]
    public TextMeshProUGUI playerKillsText;
    public TextMeshProUGUI towerKillsText;
    public TextMeshProUGUI totalKillsText;

    [Header("Zebrane Surowce")]
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI stoneText;
    public TextMeshProUGUI goldText;

    // Start wywo³a siê automatycznie zaraz po za³adowaniu sceny Game Over
    void Start()
    {
        // Sprawdzamy, czy GameManager istnieje (zabezpieczenie na wypadek testowania samej sceny Game Over)
        if (GameManager.I == null)
        {
            Debug.LogError("Brak GameManagera na scenie!");
            return;
        }

        // Dla wygody tworzymy lokalne odniesienie do statystyk z GameManagera
        GameManager.EndStats stats = GameManager.I.endStats;

        // Przypisujemy pobrane dane bezpoœrednio do komponentów tekstowych
        timeText.text = "Czas: " + stats.GetFormattedTime();
        towersBuiltText.text = "Postawione wie¿e: " + stats.towersBuilt;

        playerKillsText.text = "Zabici przez Ciebie: " + stats.enemiesKilledByPlayer;
        towerKillsText.text = "Zabici przez wie¿e: " + stats.enemiesKilledByTowers;
        totalKillsText.text = "Suma zabitych: " + stats.TotalEnemiesKilled;

        woodText.text = stats.woodCollected.ToString();
        stoneText.text = stats.stoneCollected.ToString();
        goldText.text = stats.goldCollected.ToString();
    }
}