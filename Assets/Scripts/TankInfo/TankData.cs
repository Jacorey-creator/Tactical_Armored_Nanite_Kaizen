using UnityEngine;

[CreateAssetMenu(fileName = "TankData", menuName = "Scriptable Objects/TankData")]
public class TankData : ScriptableObject
{
    public string tank_name;
    public TankTypes tankType;
    public FiringStrategyType firing_strategy;
    public TankControllers movement_strategy;
    public ProjectileData projectileData;

    public float health;
    public float armor;
    public float armor_defense;
    public float speed;
    public float fire_rate;
    public GameObject tank_prefab;
    public GameObject projectile_prefab;

    [Header("Enemies")]
    public float nav_acceleration = 0;
    public float nav_angularSpeed = 0;
    public float nav_stoppingDistance = 0;
    public float firing_distance = 0;
}
