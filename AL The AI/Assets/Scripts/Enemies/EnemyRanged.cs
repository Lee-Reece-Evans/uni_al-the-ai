using UnityEngine;
using UnityEngine.AI;

public class EnemyRanged : Enemy_Base, IStunnable
{
    public bool isStunned = false;

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
