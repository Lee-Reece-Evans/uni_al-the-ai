using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour, IPooledObject
{
    public string poolTag;
    private TextMeshPro damage;
    private Transform cameraPos;

    private void Awake()
    {
        damage = GetComponent<TextMeshPro>();
        cameraPos = Camera.main.transform;
    }

    public void SetupText(int _damage)
    {
        // set text size
        float dist = Vector3.Distance(cameraPos.position, transform.position);
        dist = Mathf.Clamp(dist, 1, 5);
        damage.fontSize = dist;

        // set text
        damage.text = _damage.ToString();

        //look at camera
        transform.LookAt(cameraPos.position);
        transform.Rotate(0f, -180f, 0f);
    }

    // animation event
    public void ReturnToPool()
    {
        ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
    }

    public void SetPoolDetails(string tag)
    {
        poolTag = tag;
    }
}
