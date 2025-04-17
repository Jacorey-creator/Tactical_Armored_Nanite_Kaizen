using UnityEngine;

public class PlayerFiringHandler : IHandleFiring
{
    private float lastFireTime;

    public void HandleFiring(TankController tank)
    {
        if (Input.GetButton("Fire1") && Time.time >= lastFireTime + tank.TankData.fire_rate)
        {
            tank.SetFiringStrategy(FiringStrategyFactory.GetStrategy(tank.TankData.firing_strategy));
            tank.GetFiringStrategy()?.Fire(tank);
            lastFireTime = Time.time;
        }
    }
}
