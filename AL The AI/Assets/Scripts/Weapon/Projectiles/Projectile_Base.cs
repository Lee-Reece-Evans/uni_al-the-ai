using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Base : MonoBehaviour, IPooledObject
{
    public float speed;
    public int damage;
    public DamageTypes damageType;
    public string poolTag;

    protected Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetPoolDetails(string tag)
    {
        poolTag = tag;
    }

    protected virtual void OnEnable()
    {
        rb.velocity = transform.forward * speed;
        StartCoroutine("OutOfRange");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // make compare tag of enemy?
        StopCoroutine("OutOfRange");

        PlayImpact(collision.GetContact(0).point);

        DealDamage(collision);

        ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
    }

    public virtual void DealDamage(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponentInParent<IDamageable>();

        if (damageable != null)
            damageable.TakeDamage(damage, damageType);
    }

    public virtual void PlayImpact(Vector3 point)
    {
    }

    IEnumerator OutOfRange()
    {
        yield return new WaitForSeconds(4f);
        ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
    }
}
