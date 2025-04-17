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
            agent.speed = tankController.TankData.speed;
            agent.acceleration = tankController.TankData.nav_acceleration;
            agent.angularSpeed = tankController.TankData.nav_angularSpeed;
            agent.stoppingDistance = tankController.TankData.nav_stoppingDistance;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[AITankController] Failed to configure agent for {gameObject.name}: {ex.Message}");
        }
    }
}