using UnityEngine;

public class PlayerFiringHandler : IHandleFiring
{
    private float fireCooldown = 0f;

    public void HandleFiring(TankController tank)
    {
        // Reduce cooldown over time
        if (fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime;
        }

        if (Input.GetButton("Fire1"))
        {
            if (fireCooldown <= 0f)
            {
                tank.SetFiringStrategy(FiringStrategyFactory.GetStrategy(tank.TankData.firing_strategy));
                tank.GetFiringStrategy()?.Fire(tank);
                fireCooldown = tank.TankData.fire_rate;
            }
            else
            {
                Debug.Log($"Reloading... {fireCooldown:F2}s remaining");
            }
        }
    }
}
