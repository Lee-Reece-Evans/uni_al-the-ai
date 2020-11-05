using UnityEngine;

public class BulletController : Projectile_Base
{
    public string impactTag;
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
}
