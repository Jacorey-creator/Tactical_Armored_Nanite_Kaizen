using UnityEngine;
using UnityEngine.AI;

public class AIMovementStrategy : ITankMovementStrategy
{
    private NavMeshAgent agent;
    private Transform target;

    public AIMovementStrategy(NavMeshAgent agent, Transform target)
    {
        this.agent = agent;
        this.target = target;
    }

    public void Move(TankController tank)
    {
        if (target == null) return;

        agent.SetDestination(target.position);

        // Optional rotation to face target
        Vector3 direction = target.position - tank.transform.position;
        direction.y = 0;
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            tank.transform.rotation = Quaternion.Slerp(tank.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}
