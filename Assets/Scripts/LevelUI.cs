using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Animator))]
public class LevelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Animator healthTextAnimator;
    [SerializeField] private TextMeshProUGUI enemiesKilledText;
    [SerializeField] private Animator enemiesKilledTextAnimator;
    [SerializeField] private TextMeshProUGUI woodCountText;
    [SerializeField] private Animator woodCountTextAnimator;
    [SerializeField] private TextMeshProUGUI stoneCountText;
    [SerializeField] private Animator stoneCountTextAnimator;
    [SerializeField] private TextMeshProUGUI goldCountText;
    [SerializeField] private Animator goldCountTextAnimator;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Animator timeTextAnimator;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private Animator waveTextAnimator;
    [Space]
    [SerializeField] private TextMeshProUGUI waveInfoText;
    private Level level;
    private Player player;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        
        ResourceManager.instance.OnWoodChanged += UpdateWood;
        ResourceManager.instance.OnStoneChanged += UpdateStone;
        ResourceManager.instance.OnGoldChanged += UpdateGold;
        UpdateWood(ResourceManager.instance.wood);
        UpdateStone(ResourceManager.instance.stone);
        UpdateGold(ResourceManager.instance.gold);

        EnemyBehaviour.OnEnemiesListChanged += UpdateEnemyCount;
        UpdateEnemyCount(EnemyBehaviour.AllEnemies.Count);

        player = FindAnyObjectByType<Player>();
        Assert.IsNotNull(player, "Player component not found in the scene. Health will not be displayed.");
        player.OnHealthChanged += UpdateHealth;
        UpdateHealth(GameManager.I.playerStats.maxHealth);

        level = FindAnyObjectByType<Level>();
        Assert.IsNotNull(level, "Level component not found in the scene. Wave info will not be displayed.");
        level.OnWaveChanged += UpdateWave;
        waveText.text = $"{0} / {level.WaveCount}";

        InvokeRepeating(nameof(UpdateTime), 0f, 1f);
    }

    private void UpdateWood(int wood)
    {
        woodCountText.text = wood.ToString();
        woodCountTextAnimator.SetTrigger("Pop");
    }

    private void UpdateStone(int stone)
    {
        stoneCountText.text = stone.ToString();
        stoneCountTextAnimator.SetTrigger("Pop");
    }

    private void UpdateGold(int gold)
    {
        goldCountText.text = gold.ToString();
        goldCountTextAnimator.SetTrigger("Pop");
    }

    private void UpdateTime()
    {
        int minutes = Mathf.FloorToInt(Time.timeSinceLevelLoad / 60f);
        int seconds = Mathf.FloorToInt(Time.timeSinceLevelLoad % 60f);
        timeText.text = $"{minutes:00}:{seconds:00}";
        timeTextAnimator.SetTrigger("Pop");
    }

    private void UpdateWave(int waveIndex)
    {
        int waveNumber = waveIndex + 1;
        waveText.text = $"{waveNumber} / {level.WaveCount}";
        waveInfoText.text = $"Fala {waveNumber} z {level.WaveCount}";
        animator.SetTrigger("StartWave");
        waveTextAnimator.SetTrigger("Pop");
    }

    private void UpdateEnemyCount(int count)
    {
        enemiesKilledText.text = count.ToString();
        enemiesKilledTextAnimator.SetTrigger("Pop");
    }

    private void UpdateHealth(float currentHealth)
    {
        healthText.text = currentHealth.ToString();
        healthTextAnimator.SetTrigger("Pop");
    }

    void OnDestroy()
    {
        ResourceManager.instance.OnWoodChanged -= UpdateWood;
        ResourceManager.instance.OnStoneChanged -= UpdateStone;
        ResourceManager.instance.OnGoldChanged -= UpdateGold;
        EnemyBehaviour.OnEnemiesListChanged -= UpdateEnemyCount;
        if (level != null)
        {
            level.OnWaveChanged -= UpdateWave;
        }
        if (player != null)
        {
            player.OnHealthChanged -= UpdateHealth;
        }
        CancelInvoke(nameof(UpdateTime));
    }
}