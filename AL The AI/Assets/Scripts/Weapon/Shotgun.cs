using UnityEngine;

public class Shotgun : Weapon_Base
{
    [SerializeField] private int numOfBullets;
    [SerializeField] private int spreadAngle;

    public override void PrimaryShot()
    {
        base.PrimaryShot();

        for (int i = 0; i<numOfBullets; i++)
        {
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

                Quaternion randRotation = Random.rotation;

                shot.transform.position = muzzle.position;
                shot.transform.rotation = Quaternion.RotateTowards(muzzle.rotation, randRotation, spreadAngle);
                shot.SetActive(true);
            }
        }
    }
}
