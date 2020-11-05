using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine_Explosive : MonoBehaviour, IPooledObject
{
    [Header("Damage Details")]
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;
    [SerializeField] private DamageTypes damageType;
    [SerializeField] private float radius = 5f;

    [Header("Pool Details")]
    [SerializeField] private string poolTag;
    [SerializeField] private string projectileTag;

    private int enemyLayer;

    private void Start()
    {
        enemyLayer = (1 << LayerMask.NameToLayer("Enemies"));
    }

    public void SetPoolDetails(string tag)
    {
        poolTag = tag;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemy"))
            Explode();
    }

    void Explode()
    {
        GameObject impact = ObjectPool.Instance.SpawnFromPool(projectileTag); // replace with bigger explosion

        if (impact != null)
        {
            impact.transform.position = transform.position;
            impact.transform.rotation = Quaternion.identity;
            impact.SetActive(true);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, enemyLayer);

        foreach (Collider enemy in colliders)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(Random.Range(minDamage, maxDamage +1), damageType);
            }

            IStunnable stunnable = enemy.GetComponent<IStunnable>();
            if (stunnable != null)
            {
                stunnable.Stun();
            }
        }

        ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
    }
}
