using UnityEngine;
using UnityEngine.AI;

public class EnemyMelee : Enemy_Base, IStunnable
{
    public bool isStunned = false;
    public string impactTag;

    public override void Shoot()
    {
        GameObject impact = ObjectPool.Instance.SpawnFromPool(impactTag);

        if (impact != null)
        {
            impact.transform.position = muzzlePos.position;
            impact.transform.rotation = muzzlePos.rotation;
            impact.SetActive(true);
        }

        if (damageableStructure != null)
            damageableStructure.TakeDamage(Random.Range(minDamage, maxDamage + 1), damagetype);

        audioSource.Play();
    }

    public void Stun()
    {
        if (health.currentHealth > 0)
        {
            isStunned = true;
            anim.SetTrigger("stunned");
        }
    }

    public void NotStunned()
    {
        isStunned = false;
    }
}
