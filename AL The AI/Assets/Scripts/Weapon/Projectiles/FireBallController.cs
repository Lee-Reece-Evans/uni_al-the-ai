using UnityEngine;

public class FireBallController : Projectile_Base
{
    public string impactTag;
    public float radius = 5f;

    public override void PlayImpact(Vector3 point)
    {
        GameObject impact = ObjectPool.Instance.SpawnFromPool(impactTag);

        if (impact != null)
        {
            impact.transform.position = point;
            impact.transform.rotation = Quaternion.identity;
            impact.SetActive(true);
        }
    }

    public override void DealDamage(Collision collision)
    {
        if (damageType == DamageTypes.Explosive)
        {
            base.DealDamage(collision);
            return;
        }

        // get colliders of enemies
        int enemyLayer = enemyLayer = (1 << LayerMask.NameToLayer("Enemies"));

        // for fire damage type - area damage
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, enemyLayer);

        foreach (Collider enemy in colliders)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage, damageType);
            }
        }
    }
}
