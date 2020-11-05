using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Turret_Base : MonoBehaviour, IDamageable, IRepairable, IPooledObject
{
    public delegate void TurretDestroyed(GameObject turret); // delegate to announce enemy has died
    public static event TurretDestroyed DestroyedEvent; // event for 

    public TurretDetails turretDetails;

    protected List<Enemy_Base> enemies = new List<Enemy_Base>();
    protected Transform closestEnemy;

    [Header("Components")]
    public Animator anim;
    public AudioSource audio;

    [Header("GameObjects")]
    public string poolTag;
    public string projectileTag;
    public string baseModelFakeTag;
    public GameObject gunRotation;
    public GameObject shield;
    public Transform[] muzzle;
    public ParticleSystem[] muzzleFlash;

    public GameObject lightModel;
    private Material lightMaterial;

    [Header("X-axis Rotation Limits 0 to 90")]
    public float elevation;
    public float depression;
    private float maxX;
    private float minX;

    [Header("Dynamic Variables")]
    public float fireTime;
    public bool hasShield;
    public float shieldHealth;
    public string shootAnimation;
    public TextMeshPro healthBar;

    [Header("Turret State")]
    public bool isBroken = false;
    public DamageTypes currentDamageType;
    public int currentDamage;

    protected int enemyLayer;
    private bool IsShowingText = false;

    private Health health;
    protected bool initialised = false;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        health = GetComponent<Health>();
        shieldHealth = turretDetails.shieldHealth;

        if (!string.IsNullOrEmpty(turretDetails.upgradePrefabTag)) // only set damage for non-upgrade turrets, upgraded models will be passed previous turrets damage type to continue behaviour.
            currentDamageType = turretDetails.ammoTypeOne;

        currentDamage = Random.Range(turretDetails.ammoOneMinDamage, turretDetails.ammoOneMaxDamage + 1);
        this.transform.Find("RangeTrigger").GetComponent<SphereCollider>().radius = turretDetails.range * 10;

        Enemy_Base.DeathEvent += EnemyDied;
        InvokeRepeating("FindEnemy", 0f, 0.5f);
        SetHealthText();
        healthBar.enabled = false;

        minX = depression;
        maxX = elevation;

        lightMaterial = lightModel.GetComponent<Renderer>().material;

        enemyLayer = 1 << LayerMask.NameToLayer("Enemies") | 1 << LayerMask.NameToLayer("Ground");

        initialised = true;
    }

    private void OnEnable()
    {
        if (!initialised)
            return;

        Enemy_Base.DeathEvent += EnemyDied;

        InvokeRepeating("FindEnemy", 0f, 0.5f); // is set first time in start
    }

    protected virtual void OnDisable()
    {
        if (!initialised)
            return;

        Enemy_Base.DeathEvent -= EnemyDied; // unsubscrive from enemy died event

        if (DestroyedEvent != null)
            DestroyedEvent(this.gameObject);

        // turret has been destroyed by enemies so put the buildable turret in its place;
        if (isBroken)
        {
            // close  menu if i had it open
            if (IngameMenuManager.instance.turretUI.GetTurret() == this)
                if (IngameMenuManager.instance.turretMenuObj.activeSelf)
                    IngameMenuManager.instance.CloseTurretMenu();

            // get a fake from the pool
            GameObject fakeTurret = ObjectPool.Instance.SpawnFromPool(baseModelFakeTag);

            if (fakeTurret != null)
            {
                fakeTurret.transform.position = transform.position;
                fakeTurret.transform.rotation = transform.rotation;
                fakeTurret.GetComponent<Structure_Placement>().initialised = true;
                fakeTurret.GetComponent<Structure_Placement>().isPlaced = true;
                fakeTurret.SetActive(true);
            }
        }

        //reset my variables
        if (hasShield) // if i have a shield at time of being disabled. remove it so that i do not start with it again.
            ActivateShield();

        CancelInvoke();
        isBroken = false;
        closestEnemy = null;
        enemies.Clear();
        health.currentHealth = health.maxHealth;
        currentDamageType = turretDetails.ammoTypeOne;
    }

    public void SetPoolDetails(string tag)
    {
        poolTag = tag;
    }

    public void ActivateShield()
    {
        hasShield = !hasShield;
        shieldHealth = turretDetails.shieldHealth;
        shield.SetActive(!shield.activeSelf);
    }

    public void TakeDamage(int damage, DamageTypes _damageType)
    {
        if (hasShield)
        {
            shieldHealth -= damage;
            ShowDamagePopup(transform.position + Vector3.up * 2.2f, damage);

            if (shieldHealth <= 0)
                ActivateShield();

            return;
        }

        if (health.currentHealth > 0)
        {
            health.currentHealth -= damage;
            ShowDamagePopup(transform.position + Vector3.up * 2.2f, damage);

            if (health.currentHealth <= 0)
            {
                health.currentHealth = 0;

                if (!isBroken)
                {
                    isBroken = true;

                    // put me back into the pool
                    ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
                }
            }
            SetHealthText();
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

    public void Repair(int amount)
    {
        if (!IsShowingText) // show health text
        {
            healthBar.enabled = true;
            StartCoroutine("HideHealthText");
        }
        else
        {
            StopCoroutine("HideHealthText");
            StartCoroutine("HideHealthText");
        }

        if (health.currentHealth < health.maxHealth)
        {
            health.currentHealth += amount;

            if (health.currentHealth >= health.maxHealth)
            {
                health.currentHealth = health.maxHealth;
            }
            SetHealthText();
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemy"))
        {
            Enemy_Base enemy = other.gameObject.GetComponent<Enemy_Base>();

            enemies.Add(enemy);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("enemy"))
        {
            if (other.transform == closestEnemy)
                closestEnemy = null;

            Enemy_Base enemy = other.gameObject.GetComponent<Enemy_Base>();

            enemies.Remove(enemy);
        }
    }

    public void EnemyDied(Enemy_Base enemy) // for delegate event remove enemy from list when died
    {
        if (!enemies.Count.Equals(0))
        {
            if (enemies.Contains(enemy))
            {
                enemies.Remove(enemy);

                if (enemies.Count == 0 || closestEnemy == enemy.gameObject.transform) // GET RID OF REFERENCE TO CLOSEST ENEMY
                    closestEnemy = null;
            }
        }
    }

    private bool CanSeeEnemy(Transform enemy)
    {
        if (Physics.Raycast(transform.position + Vector3.up, ((enemy.position + Vector3.up) - (transform.position + Vector3.up)), out RaycastHit Hit, Mathf.Infinity, enemyLayer, QueryTriggerInteraction.Ignore))
        {
            if (Hit.transform.root.gameObject.CompareTag("enemy"))
            {
                return true;
            }
        }
        return false;
    }

    private void FindEnemy()
    {
        if (enemies.Count != 0)
        {
            float shortestDistance = Mathf.Infinity;

            foreach (Enemy_Base enemy in enemies)
            {
                if (enemy == null)
                {
                    Debug.Log("was null");
                    return;
                }

                if (enemy.isInvisible)
                {
                    if (closestEnemy == enemy.gameObject.transform)
                    {
                        closestEnemy = null;
                    }
                    continue; // skip this one
                }

                if (closestEnemy != null && !CanSeeEnemy(closestEnemy))
                    closestEnemy = null;

                float distance = Vector3.Distance(transform.position, enemy.gameObject.transform.position);

                if (distance < shortestDistance)
                {
                    if (CanSeeEnemy(enemy.gameObject.transform))
                    {
                        shortestDistance = distance;
                        closestEnemy = enemy.gameObject.transform;
                    }
                }
            }
        }
    }

    private void CanShoot()
    {
        if (fireTime <= 0)
        {
            Shoot();
            fireTime = turretDetails.fireRate;
        }
    }

    protected virtual void Shoot()
    {
        if (currentDamageType == turretDetails.ammoTypeOne)
        {
            currentDamage = Random.Range(turretDetails.ammoOneMinDamage, turretDetails.ammoOneMaxDamage + 1); ;
        }
        else if (currentDamageType == turretDetails.ammoTypeTwo)
        {
            currentDamage = Random.Range(turretDetails.ammoTwoMinDamage, turretDetails.ammoTwoMaxDamage + 1); ;
        }

        // override in sub classes
    }

    private void MuzzleFlash(int muzzlePos)
    {
        // used by turrets fire animation event. inputs the muzzle at array position
        muzzleFlash[muzzlePos].Play();
    }

    public void Upgrade()
    {
        GameObject upgradeTurret = ObjectPool.Instance.SpawnFromPool(turretDetails.upgradePrefabTag);
        upgradeTurret.transform.position = transform.position;
        upgradeTurret.transform.rotation = transform.rotation;
        upgradeTurret.SetActive(true);

        Turret_Base upgradeTurretBase = upgradeTurret.GetComponent<Turret_Base>();

        if (upgradeTurretBase != null)
        {
            upgradeTurretBase.currentDamageType = currentDamageType;

            if (hasShield)
                upgradeTurretBase.ActivateShield();
        }

        ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
    }

    public void Refund()
    {
        ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
    }

    public void SetAmmoTypeOne()
    {
        currentDamageType = turretDetails.ammoTypeOne;
    }

    public void SetAmmoTypeTwo()
    {
        currentDamageType = turretDetails.ammoTypeTwo;
    }

    private bool AimAtEnemy()
    {
        Vector3 direction = ((closestEnemy.position + Vector3.up) - gunRotation.transform.position).normalized;
        Quaternion lookatrotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(gunRotation.transform.rotation, lookatrotation, Time.deltaTime * turretDetails.turnSpeed).eulerAngles;

        if (rotation.x > 90)
        {
            minX = 360 - elevation;
            maxX = 360;
        }
        if (rotation.x < 90)
        {
            minX = 0;
            maxX = depression;
        }
        rotation.x = Mathf.Clamp(rotation.x, minX, maxX);

        gunRotation.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0f);

        // check the angle between the turret barrel and enemy is enough so that a shot will be accurate when calling shoot().
        if (Vector3.Angle(gunRotation.transform.forward, direction) < 2.5) // within 2.5 degrees
            return true;

        return false;
    }

    private void SetHealthText()
    {
        float healthPercentage = ((float)health.currentHealth / (float)health.maxHealth) * 100f;
        healthBar.text = (int)healthPercentage + " %";

        if (healthPercentage <= 25f)
            healthBar.color = Color.red;
        else if (healthPercentage <= 60f)
            healthBar.color = Color.yellow;
        else
            healthBar.color = Color.green;
    }

    IEnumerator HideHealthText()
    {
        IsShowingText = true;
        yield return new WaitForSeconds(3f);
        healthBar.enabled = false;
        IsShowingText = false;
    }

    void Update()
    {
        if (fireTime > 0)
            fireTime -= Time.deltaTime;

        if (closestEnemy != null && !isBroken)
        {
            if (AimAtEnemy())
                CanShoot();
        }
    }
}
