using UnityEngine;

public class AIFiringHandler : IHandleFiring
{
    private float lastFireTime;

    public void HandleFiring(TankController tank)
    {
        if (tank.GetAITarget() == null) return;

        float distanceToTarget = Vector3.Distance(tank.transform.position, tank.GetAITarget().position);
        if (distanceToTarget < tank.TankData.firing_distance && Time.time >= lastFireTime + tank.TankData.fire_rate)
        {
            tank.SetFiringStrategy(FiringStrategyFactory.GetStrategy(tank.TankData.firing_strategy));
            tank.GetFiringStrategy()?.Fire(tank);
            lastFireTime = Time.time;
        }
    }
}
