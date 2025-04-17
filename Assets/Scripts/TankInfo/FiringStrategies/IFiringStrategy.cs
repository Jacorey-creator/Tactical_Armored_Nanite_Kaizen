using UnityEngine;

public interface IFiringStrategy
{
    void Fire(TankController tank);
}

public enum FiringStrategyType
{
    SingleShot,
    SpreadShot
}

public class SingleShotStrategy : IFiringStrategy
{
    public void Fire(TankController tank)
    {
        // Validate required data
        if (tank.TankData.projectile_prefab == null)
        {
            Debug.LogWarning("No projectile assigned in TankData!");
            return;
        }

        Transform firePoint = tank.GetFiringPoint();
        if (firePoint == null)
        {
            Debug.LogWarning("No firing point found on the tank!");
            return;
        }

        // Determine the direction the tank is facing
        Vector3 fireDirection = tank.transform.forward;

        // Instantiate the projectile
        GameObject projectile = GameObject.Instantiate(
            tank.TankData.projectile_prefab,
            firePoint.position,
            firePoint.rotation
        );

        // Apply force using the projectile's Rigidbody
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;  // Reset velocity
            rb.AddForce(fireDirection * tank.TankData.projectileData.velocity, ForceMode.VelocityChange);
        }
        else
        {
            Debug.LogWarning("Projectile does not have a Rigidbody!");
        }

        // Initialize the projectile's behavior
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(tank.TankData.projectileData);
        }
        else
        {
            Debug.LogWarning("Projectile script missing on instantiated projectile!");
        }
    }
}

public class SpreadShotStrategy : IFiringStrategy
{
    public void Fire(TankController tank)
    {
        // Retrieve the firing point and validate required data
        Transform firePoint = tank.GetFiringPoint();
        if (tank.TankData.projectile_prefab == null || tank.TankData.projectileData == null || firePoint == null)
        {
            Debug.LogWarning("Missing projectile prefab, projectile data, or fire point on tank!");
            return;
        }

        ProjectileData data = tank.TankData.projectileData;
        float spreadAngle = 15f;

        // Fire three projectiles in a spread: left, center, right
        for (int i = -1; i <= 1; i++)
        {
            Quaternion spreadRotation = firePoint.rotation * Quaternion.Euler(0, i * spreadAngle, 0);
            GameObject projectileGO = GameObject.Instantiate(
                tank.TankData.projectile_prefab,
                firePoint.position,
                spreadRotation
            );

            Projectile projectile = projectileGO.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Initialize(data);
            }
            else
            {
                Debug.LogWarning("Projectile script missing on instantiated projectile in spread shot!");
            }
        }
    }
}