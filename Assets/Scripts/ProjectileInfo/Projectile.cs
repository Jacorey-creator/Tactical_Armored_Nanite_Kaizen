using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float lifetime;
    private float speed;
    private float damage;
    private bool explodes;
    private float radius;
    private float falloff;

    public void Initialize(ProjectileData data)
    {
        speed = data.velocity;
        damage = data.damage;
        lifetime = data.lifetime;
        explodes = data.explodes;
        radius = data.explosionRadius;
        falloff = data.damageFalloff;

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Projectile collided with: " + collision.gameObject.name);
        if (explodes)
 Explode();
        else
        {
            if (collision.collider.TryGetComponent(out TankController tank))
            {
                TankHealthLogic.TakeDamage(
                    tank.TankData,
                    tank,
                    damage,
                    ref tank.currentShield,
                    ref tank.currentHealth);
            }
        }
        Destroy(gameObject);
    }

    private void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        Debug.Log("Exploded! Found " + hits.Length + " colliders.");

        foreach (var hit in hits)
        {
            Debug.Log("Overlap hit: " + hit.name);
            if (hit.TryGetComponent(out TankController tank))
            {
                Debug.Log("Tank Controller hit!");
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                float damageMultiplier = 1 - Mathf.Clamp01(distance / radius) * falloff;
                float finalDamage = damage * damageMultiplier;

                TankHealthLogic.TakeDamage(
                    tank.TankData,
                    tank,
                    finalDamage,
                    ref tank.currentShield,
                    ref tank.currentHealth);
            }
        }
    }
}
