using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AITankController : MonoBehaviour
{
    private TankController tankController;
    private NavMeshAgent agent;

    private void Awake()
    {
        tankController = GetComponent<TankController>();
        agent = GetComponent<NavMeshAgent>();
        ConfigureAgentByTankType();
    }

    private void Start()
    {
        StartCoroutine(WaitForPlayerAndSetTarget());
    }

    private void ConfigureAgentByTankType()
    {
        try
        {
            // Set basic stats from TankData
            var data = tankController.TankData;
            agent.speed = data.speed;
            agent.acceleration = data.nav_acceleration;
            agent.angularSpeed = data.nav_angularSpeed;
            agent.stoppingDistance = data.nav_stoppingDistance;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[AITankController] Failed to configure agent for {gameObject.name}: {ex.Message}");
        }
    }

    private IEnumerator WaitForPlayerAndSetTarget()
    {
        GameObject player = null;

        // Wait until the player exists
        while (player == null)
        {
            player = GameObject.FindWithTag("Player");
            yield return null; // wait one frame
        }

        // Set player as target in the TankController
        tankController.SetAITarget(player.transform);

        // Assign and apply the target to the movement strategy
        SetAITarget();

        Debug.LogWarning($"Target Found");
    }

    public void SetAITarget()
    {
        Transform target = tankController.GetAITarget();

        if (tankController.TankData.movement_strategy == TankControllers.AI && target != null)
        {
            // Apply the movement strategy using the AI target
            tankController.SetMovementStrategy(new AIMovementStrategy(agent, target));
        }
        else
        {
            Debug.LogWarning($"[AITankController] No valid AI target set for {gameObject.name} or not AI strategy.");
        }
    }
}
