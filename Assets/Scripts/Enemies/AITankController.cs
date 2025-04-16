using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

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
    private void ConfigureAgentByTankType()
    {
        try
        {
            // Set basic stats from data
            agent.speed = tankController.tankData.speed;
            agent.acceleration = tankController.tankData.nav_acceleration;
            agent.angularSpeed = tankController.tankData.nav_angularSpeed;
            agent.stoppingDistance = tankController.tankData.nav_stoppingDistance;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[AITankController] Failed to configure agent for {gameObject.name}: {ex.Message}");
        }
    }
}