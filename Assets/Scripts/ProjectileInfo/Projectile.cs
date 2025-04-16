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
        if (explodes)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, radius);
            foreach (var hit in hits)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                float damageMultiplier = 1 - Mathf.Clamp01(distance / radius) * falloff;

                // Apply damage based on your damage system
                // Example: hit.GetComponent<Health>()?.TakeDamage(damage * damageMultiplier);
            }
        }
        else
        {
            //Apply direct damage
            //Example: collision.collider.GetComponent<Health>()?.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
