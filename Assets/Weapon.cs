using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public Sprite weaponSprite;

    [Range(0f, 5f)] public float damageMultiplier   = 1f;
    [Range(0f, 5f)] public float rangeMultiplier    = 1f;
    [Range(0f, 5f)] public float cooldownMultiplier = 1f;

    public static readonly List<string> StatNames = new List<string> { "damage", "range", "cooldown" };

    public void ApplyBoost(string stat, int percent)
    {
        float multiplier = 1f + percent / 100f;
        switch (stat.ToLower())
        {
            case "damage":   damageMultiplier   *= multiplier; break;
            case "range":    rangeMultiplier    *= multiplier; break;
            case "cooldown": cooldownMultiplier *= multiplier; break;
            default: Debug.LogWarning($"[Weapon] Nieznany stat: '{stat}'"); break;
        }
        Debug.Log($"[Weapon] {weaponName}: boost '{stat}' +{percent}% → DMG:{damageMultiplier:F2} Range:{rangeMultiplier:F2} CD:{cooldownMultiplier:F2}");
    }
}
