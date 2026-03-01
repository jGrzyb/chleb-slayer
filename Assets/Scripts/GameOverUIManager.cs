using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUIManager : MonoBehaviour
{
    [Header("G��wne Statystyki")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI towersBuiltText;

    [Header("Zab�jstwa")]
    public TextMeshProUGUI playerKillsText;
    public TextMeshProUGUI towerKillsText;
    public TextMeshProUGUI totalKillsText;

    [Header("Zebrane Surowce")]
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI stoneText;
    public TextMeshProUGUI goldText;

    public string sceneName = "NewsScene";

    // Start wywo�a si� automatycznie zaraz po za�adowaniu sceny Game Over
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

        // Przypisujemy pobrane dane bezpo�rednio do komponent�w tekstowych
        timeText.text = "Czas: " + stats.GetFormattedTime();
        towersBuiltText.text = "Postawione wie�e: " + stats.towersBuilt;

        playerKillsText.text = "Zabici przez Ciebie: " + stats.enemiesKilledByPlayer;
        towerKillsText.text = "Zabici przez wie�e: " + stats.enemiesKilledByTowers;
        totalKillsText.text = "Suma zabitych: " + stats.TotalEnemiesKilled;

        woodText.text = stats.woodCollected.ToString();
        stoneText.text = stats.stoneCollected.ToString();
        goldText.text = stats.goldCollected.ToString();
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}