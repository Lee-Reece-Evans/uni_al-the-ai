using UnityEngine;

public class Sniper : Weapon_Base
{
    public override void PrimaryShot()
    {
        base.PrimaryShot();

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit Hit, Mathf.Infinity, Enemieslayermask))
        {
            IDamageable damageEnemy = Hit.collider.GetComponent<IDamageable>();
            if (damageEnemy != null)
            {
                damageEnemy.TakeDamage(Random.Range(primaryMinDamage, primaryMaxDamage + 1), primaryDamageType);
            }
        }
    }
}
