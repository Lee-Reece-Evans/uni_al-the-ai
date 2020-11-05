using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledImpactFX : MonoBehaviour, IPooledObject
{
    public string poolTag;
    public float disableTime;
    public void SetPoolDetails(string tag)
    {
        poolTag = tag;
    }

    private void OnEnable()
    {
        StartCoroutine("ReturnToPool");
    }

    IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(disableTime);
        ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
    }
}
