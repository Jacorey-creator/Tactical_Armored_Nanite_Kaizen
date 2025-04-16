using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class TankController : MonoBehaviour
{
    public TankData tankData; // Reference to our Scriptable Object
    private float currentHealth;
    private float currentShield;
    private float lastFireTime;
    private IFiringStrategy firingStrategy;
    private ITankMovementStrategy movementStrategy;
    private IHandleFiring firingHandler;


    public MonoBehaviour inputSource; // Assigned in Inspector
    [SerializeField] private Transform cameraPivot;     // Set via Inspector if player
    [SerializeField] private Transform aiTarget;
    [SerializeField] private Transform firePoint;
    public Transform FirePoint => firePoint;
    public Transform GetAITarget() => aiTarget;
    public IFiringStrategy GetFiringStrategy() => firingStrategy;
    private void Awake()
    {
        // Initialize tank properties from the Scriptable Object data
        currentHealth = tankData.health;
        currentShield = tankData.armor;
        firingStrategy = FiringStrategyFactory.GetStrategy(tankData.firing_strategy);

        movementStrategy = MovementStrategyFactory.GetStrategy(
        tankData.movement_strategy,
        gameObject,
        inputSource,
        cameraPivot,
        aiTarget
        );

        firingHandler = HandleFiringFactory.GetHandler(tankData.movement_strategy);
    }

    private void Update()
    {
        movementStrategy?.Move(this);
        firingHandler?.HandleFiring(this);
    }
    public void SetMovementStrategy(ITankMovementStrategy strategy)
    {
        movementStrategy = strategy;
    }
    public void SetFiringStrategy(IFiringStrategy strategy)
    {
        firingStrategy = strategy;
    }
    public Transform GetFiringPoint() 
    {
        return FirePoint;
    }
    private float DefenseCalculation(float damage) 
    {
        // Ensure the armor defense value is clamped between 0 and 100.
        float clampedArmorDefense = Mathf.Clamp(tankData.armor_defense, 0f, 100f);

        // Calculate damage reduction percentage.
        float damageReductionPercent = clampedArmorDefense / 100f;

        // Compute the effective damage after the reduction.
        return damage * (1f - damageReductionPercent);
    }
    public void TakeDamage(float damage)
    {
        if (currentShield > 0) 
        {
            currentShield -= DefenseCalculation(damage);
        }
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            // Handle destruction or respawn
            Destroy(gameObject);
        }
    }
    public void SetAITarget(Transform target)
    {
        aiTarget = target;

        if (tankData.movement_strategy == TankControllers.AI)
        {
            var agent = GetComponent<NavMeshAgent>();
            movementStrategy = new AIMovementStrategy(agent, aiTarget);
        }
    }
}
