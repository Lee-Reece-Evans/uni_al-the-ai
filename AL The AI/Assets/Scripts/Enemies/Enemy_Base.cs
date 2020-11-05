using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Base : MonoBehaviour, IDamageable, IPooledObject
{
    public delegate void EnemyDeath(Enemy_Base enemy); // delegate to announce enemy has died
    public static event EnemyDeath DeathEvent; // event for 

    [Header("Pooled Details")]
    public string projectileTag;
    public string poolTag;

    [Header("Damage Details")]
    public DamageTypes weakness;
    public DamageTypes damagetype;
    public int minDamage;
    public int maxDamage;
    public float accuracyOffsetAngle; // angle within - can be improved as waves progress...
    public int range; // for trigger

    [Header("Reward Details")]
    public int numOfMatsToDrop;

    [Header("Objective Details")]
    public Vector3 objective;
    public Vector3 closestResource;
    public string target;
    public readonly string targetPlayer = "Player";
    public readonly string targetStructure = "structure";
    public readonly string targetResource = "resource";

    [Header("Components")]
    public NavMeshAgent agent;
    public Animator anim;
    public Rigidbody rb;
    public AudioClip[] shotSounds;
    public AudioSource audioSource;
    protected Health health;
    public Transform muzzlePos;
    public GameObject shield;

    [Header("Runtime assigned")]
    public bool isDead = false;
    public bool isInvisible = false;
    public List<GameObject> objectsInRange = new List<GameObject>();
    public ResourcePoint resPoint; // reference to the enemies closest resourcepoint script
    public IDamageable damageableStructure; // for melee attackers

    private bool initialised = false;

    public void SetPoolDetails(string tag)
    {
        poolTag = tag;
    }

    private void Start()
    {
        //EnemiesManager.instance.enemies.Add(this); // for testing
    }

    public void TakeDamage(int damage, DamageTypes _damageType)
    {
        if (health.currentHealth > 0)
        {
            if (weakness.Equals(_damageType))
            {
                damage *= 2;
                health.currentHealth -= damage; // take double damage if weak to damage type
            }
            else
            {
                health.currentHealth -= damage; // take regular damage amount if damagetype has no effect
            }

            ShowDamagePopup(transform.position + Vector3.up * 2.2f, damage);

            if (health.currentHealth <= 0)
            {
                health.currentHealth = 0;
                Die();
            }
        }
    }

    private void ShowDamagePopup(Vector3 point, int damage)
    {
        GameObject damagePopup = ObjectPool.Instance.SpawnFromPool("DamagePopup");

        if (damagePopup != null)
        {
            Vector3 randomisedPos = new Vector3(Random.Range(-0.5f, 0.5f), 0, 0);

            damagePopup.transform.position = point + randomisedPos;
            damagePopup.GetComponent<DamagePopup>().SetupText(damage);
            damagePopup.SetActive(true);
        }
    }

    private void Awake()
    {
        health = GetComponent<Health>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (!initialised)
        {
            initialised = true;
            return;
        }

        isDead = false; // enemiesManager checks this when iterating through its list to know which to include.
        EnemiesManager.instance.enemies.Add(this);

        Turret_Base.DestroyedEvent += TurretDestroyed; // subscribe to turret destroyed event

        health.currentHealth = health.maxHealth;

        if (shield != null)
            shield.SetActive(true);
    }

    private void OnDisable()
    {
        Turret_Base.DestroyedEvent -= TurretDestroyed; // unsubscribe from turret destroyed event

        objectsInRange.Clear();
        resPoint = null;
        closestResource = Vector3.zero;
        objective = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetStructure))
        {
            objectsInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetStructure))
        {
            objectsInRange.Remove(other.gameObject);
        }
    }

    public virtual void Shoot() // accessed by animation event shoot.
    {
        GameObject shot = ObjectPool.Instance.SpawnFromPool(projectileTag);

        if (shot != null)
        {
            Projectile_Base shotDetails = shot.GetComponent<Projectile_Base>();

            if (shotDetails != null)
            {
                shotDetails.damageType = damagetype;
                shotDetails.damage = Random.Range(minDamage, maxDamage + 1);
            }

            audioSource.clip = shotSounds[Random.Range(0, shotSounds.Length)]; // choose random shot sound
            audioSource.Play(); // shot sound

            Quaternion randRotation = Random.rotation;

            shot.transform.position = muzzlePos.position;
            shot.transform.rotation = Quaternion.RotateTowards(muzzlePos.rotation, randRotation, accuracyOffsetAngle);
            shot.SetActive(true);
        }
    }

    public void Die() // accessed by health component
    {
        isDead = true;

        if (DeathEvent != null)
            DeathEvent(this); // send to subscribers of delegate event

        if (shield != null)
            shield.SetActive(false);

        anim.SetTrigger("die");
    }

    public void DestroyEnemy() // accessed by animation event die
    {
        DropMaterials();
        ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
    }

    private void DropMaterials()
    {
        for (int i = 0; i < numOfMatsToDrop; i++)
        {
            int rand = Random.Range(0, 2); // between 0 and 1
            Vector3 offsetVector = -transform.forward + (Vector3.up * 0.2f);

            GameObject material;

            if (rand == 0)
                material = ObjectPool.Instance.SpawnFromPool("PickupBolt");
            else
                material = ObjectPool.Instance.SpawnFromPool("PickupNut");

            if (material != null)
            {
                material.transform.position = transform.position + offsetVector;
                material.transform.rotation = Quaternion.identity;
                material.SetActive(true);
            }
        }
    }

    public void TurretDestroyed(GameObject turret)
    {
        if (!objectsInRange.Count.Equals(0)) // if list is not empty 
        {
            if (objectsInRange.Contains(turret)) // check if turret is contained in list
            {
                objectsInRange.Remove(turret); // remove from list
            }
        }
    }

    public void GatheredResource()
    {
        if (resPoint != null)
        {
            resPoint.TakeResource(1);
        }
    }
}
