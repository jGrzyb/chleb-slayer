using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUIManager : MonoBehaviour
{

    [Header("Główne Statystyki")]
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

    [Header("Wiadomości")]
    public TextMeshProUGUI newsText;

    public string sceneName = "NewsScene";

    void Start()
    {
        SoundManager.I.PlayMusic(SoundManager.I.NewsMusic);
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
        towersBuiltText.text = "Postawione wieże: " + stats.towersBuilt;

        playerKillsText.text = "Zabici przez Ciebie: " + stats.enemiesKilledByPlayer;
        towerKillsText.text = "Zabici przez wieże: " + stats.enemiesKilledByTowers;
        totalKillsText.text = "Suma zabitych: " + stats.TotalEnemiesKilled;

        woodText.text = stats.woodCollected.ToString();
        stoneText.text = stats.stoneCollected.ToString();
        goldText.text = stats.goldCollected.ToString();
        if (stats.win)
        {
            newsText.text = "PILNE: Zeszłej nocy piekarnia przetrwała oblężenie hordy! Bohaterski piekarz przepędził demony z powrotem do otchłani. Piekło zostało wypieczone na chrupiąco!";
        }
        else
        {
            newsText.text = "PILNE: Zeszłej nocy horda piekielna obróciła lokalną piekarnię w popiół. Piekarz stracił wszystko, a miasto obudził zapach spalenizny zamiast świeżego chleba. Ciemność zwyciężyła.";
        }
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}