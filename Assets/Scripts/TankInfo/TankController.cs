using System;
using UnityEngine;

public class TankController : MonoBehaviour
{
    [SerializeField] private TankData tankData;
    public float currentHealth;
    public float currentShield;
    private float lastFireTime;
    private IFiringStrategy firingStrategy;
    private ITankMovementStrategy movementStrategy;
    private IHandleFiring firingHandler;


    public MonoBehaviour inputSource; // Assigned in Inspector
    [SerializeField] private Transform aiTarget;
    [SerializeField] private Transform firePoint;
    public Transform GetAITarget() => aiTarget;
    public IFiringStrategy GetFiringStrategy() => firingStrategy;
    public TankData TankData => tankData;
    public Transform FirePoint => firePoint;

    private void Awake()
    {
        InitializeTank();
    }

    private void Update()
    {
        movementStrategy?.Move(this);
        firingHandler?.HandleFiring(this);
    }
    private void InitializeTank() 
    {
        // Initialize tank properties from the Scriptable Object data
        currentHealth = tankData.health;
        currentShield = tankData.armor;
        firingStrategy = FiringStrategyFactory.GetStrategy(tankData.firing_strategy);

        movementStrategy = MovementStrategyFactory.GetStrategy(
        tankData.movement_strategy,
        gameObject,
        inputSource,
        aiTarget
        );

        firingHandler = HandleFiringFactory.GetHandler(tankData.movement_strategy);
        SetTankVisual(tankData.tank_prefab);
    }

    public void SetMovementStrategy(ITankMovementStrategy strategy)
    {
        movementStrategy = strategy;
    }
    public void SetFiringStrategy(IFiringStrategy strategy)
    {
        firingStrategy = strategy;
    }
    public void SetTankData(TankData newData)
    {
        tankData = newData;
        currentHealth = tankData.health;
        currentShield = tankData.armor;

        firingStrategy = FiringStrategyFactory.GetStrategy(tankData.firing_strategy);
        movementStrategy = MovementStrategyFactory.GetStrategy(
            tankData.movement_strategy, gameObject, inputSource, aiTarget);
        firingHandler = HandleFiringFactory.GetHandler(tankData.movement_strategy);
    }
    public void SetAITarget(Transform target)
    {
        aiTarget = target;
    }
    public void SetTankVisual(GameObject newVisual)
    {
        TankVisualManager.SetTankVisual(gameObject, newVisual, out Transform updatedFirePoint);
        if (updatedFirePoint != null) firePoint = updatedFirePoint;
    }

}
public struct TankDamageEvent
{
    public TankController tank;
    public float damageTaken;
    public float remainingHealth;
    public float remainingShield;

    public TankDamageEvent(TankController tank, float damageTaken, float remainingHealth, float remainingShield)
    {
        this.tank = tank;
        this.damageTaken = damageTaken;
        this.remainingHealth = remainingHealth;
        this.remainingShield = remainingShield;
    }
}

public static class TankHealthLogic
{
    private static float DefenseCalculation(TankData tankData, float damage)
    {
        float clampedArmorDefense = Mathf.Clamp(tankData.armor_defense, 0f, 100f);
        float damageReductionPercent = clampedArmorDefense / 100f;
        return damage * (1f - damageReductionPercent);
    }

    public static void TakeDamage(TankData tankData, TankController tankController, float damage, ref float currentShield, ref float currentHealth)
    {
        float finalDamage = DefenseCalculation(tankData, damage);
        Debug.Log($"Tank {tankController.name} is taking {finalDamage} damage!");

        if (currentShield > 0)
        {
            currentShield -= finalDamage;
            currentShield = Mathf.Max(0, currentShield);
        }
        else
        {
            currentHealth -= finalDamage;
        }

        TankEvents.RaiseTankDamaged(new TankDamageEvent(tankController, finalDamage, currentHealth, currentShield));

        if (currentHealth <= 0)
        {
            Debug.Log("Ded");
            GameObject.Destroy(tankController.gameObject); // clean destroy
        }
    }
}