using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Scriptable Objects/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    [Header("Basic Settings")]
    public GameObject owner;
    public float velocity = 20f;
    public float damage = 10f;
    public float lifetime = 5f;

    [Header("Explosion Settings")]
    public bool explodes = false;
    public float explosionRadius = 5f;
    [Tooltip("0 = no falloff (full damage in radius), 1 = full falloff (0 damage at edge)")]
    [Range(0f, 1f)]
    public float damageFalloff = 0.5f;
}