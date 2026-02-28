using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public Sprite weaponSprite;

    [Range(0f, 5f)] public float damageMultiplier    = 1f;
    [Range(0f, 5f)] public float rangeMultiplier     = 1f;
    [Range(0f, 5f)] public float cooldownMultiplier  = 1f;
}
