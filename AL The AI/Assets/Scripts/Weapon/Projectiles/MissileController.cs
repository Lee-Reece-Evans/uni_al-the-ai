using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : Projectile_Base
{
    public float turnSpeed;
    private Transform target;
    public string impactTag;
    public bool shouldPlayImpact;

    private void OnDisable()
    {
        target = null;
        shouldPlayImpact = false;
    }

    public void SeekTarget(Transform _target)
    {
        target = _target;
    }

    void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;

        if (target != null)
        {
            Vector3 adjustment = target.position + Vector3.up;
            Quaternion lookRot = Quaternion.LookRotation(adjustment - transform.position);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, lookRot, turnSpeed));
        }
    }

    public override void PlayImpact(Vector3 point)
    {
        if (!shouldPlayImpact)
            return;

        GameObject impact = ObjectPool.Instance.SpawnFromPool(impactTag);

        if (impact != null)
        {
            impact.transform.position = transform.position;
            impact.transform.rotation = Quaternion.identity;
            impact.SetActive(true);
        }
    }
}
