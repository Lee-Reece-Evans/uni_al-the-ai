using UnityEngine;

public class Rifle : Weapon_Base
{
    public override void PrimaryShot()
    {
        base.PrimaryShot();

        GameObject shot = ObjectPool.Instance.SpawnFromPool(primaryProjectileTag);

        if (shot != null)
        {
            Projectile_Base shotDetails = shot.GetComponent<Projectile_Base>();

            if (shotDetails != null)
            {
                shotDetails.damageType = primaryDamageType;
                shotDetails.damage = Random.Range(primaryMinDamage, primaryMaxDamage + 1);
                shotDetails.speed = primarySpeed;
            }

            shot.transform.position = muzzle.position;

            RaycastHit Hit;
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out Hit, Mathf.Infinity, EverythingLayerMask, QueryTriggerInteraction.Ignore))
            {
                shot.transform.LookAt(Hit.point);
            }
            else
                shot.transform.rotation = muzzle.rotation;

            shot.SetActive(true);
        }
    }
}
