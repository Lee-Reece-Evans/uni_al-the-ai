using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsPickup : MonoBehaviour, IPooledObject
{
    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private BoxCollider collider;

    public int rewardAmount;
    public string poolTag;

    private bool goToPlayer;
    private Transform player;
    private readonly string playertag = "Player";

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        goToPlayer = false;
    }

    private void OnEnable()
    {
        rb.isKinematic = false;
        collider.enabled = true;
    }

    public void SetPoolDetails(string tag)
    {
        poolTag = tag;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playertag))
        {
            goToPlayer = true;
            rb.isKinematic = true;
            collider.enabled = false;
        }
    }

    void Update()
    {
        if (goToPlayer)
        {
            if ((player.position - transform.position).sqrMagnitude > 0.5f)
                transform.position = Vector3.MoveTowards(transform.position, player.position, Time.deltaTime * 10f);
            else
            {
                SFXManager2D.instance.PlayMaterialSound();
                goToPlayer = false;
                PlayerStats.instance.AddMoney(rewardAmount);
                ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
            }
        }
    }
}
