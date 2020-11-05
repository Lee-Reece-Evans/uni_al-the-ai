using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class drone : MonoBehaviour, IInteractable, IPooledObject
{
    public string poolTag;
    [SerializeField] private float fireRate = 1f;
    private float nextFireTime = 0f;

    [Header("Components")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LineRenderer lineRenderer;

    // dynamic variables
    private List<Health> objectHealth;
    private List<IRepairable> repairableObjects;
    private IRepairable objectToRepair;
    private bool wander;
    private bool repairing;
    private Vector3 objective;
    private Vector3 rotateToObject;
    private Vector3 originalPosition;

    // initialisation variables
    private bool initialised;
    private bool onNavMesh;
    private Vector3 navMeshPos;

    private void Initialisation() // each time upon starting
    {
        InvokeRepeating("FindClosestRepairableObject", 0f, 0.5f);
        originalPosition = transform.position;
        navMeshPos = transform.position + Vector3.up * 5.4f;
        Turret_Base.DestroyedEvent += TurretDestroyed;

        agent.enabled = false;
    }

    void Start()
    {
        objectHealth = new List<Health>();
        repairableObjects = new List<IRepairable>();

        Initialisation();
        initialised = true;
    }

    private void OnEnable()
    {
        if (!initialised)
            return;

        Initialisation();
    }

    private void OnDisable()
    {
        if (!initialised)
            return;

        CancelInvoke();
        onNavMesh = false;
        wander = false;
        objective = Vector3.zero;
        objectToRepair = null;
        objectHealth.Clear();
        repairableObjects.Clear();

        Turret_Base.DestroyedEvent -= TurretDestroyed;
    }

    public void DoInteraction()
    {
        // make a menu for telling the drone what to do and pass it the drone script
        if (!IngameMenuManager.instance.shopUI.handsFull)
        {
            IngameMenuManager.instance.repairDroneUI.SetDrone(this);
        }
    }

    public void SetPoolDetails(string tag)
    {
        poolTag = tag;
    }

    public void DoInitialisationMovement()
    {
        //make drone move to navmesh position
        transform.position = Vector3.Lerp(transform.position, navMeshPos, 1 * Time.deltaTime);

        if (transform.position.y > 5.3)
        {
            agent.enabled = true;
            onNavMesh = true;
        }
    }

    public void Refund()
    {
        ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("structure"))
            return;

        Health health = other.transform.root.gameObject.GetComponent<Health>();
        if (health != null) // check if object has health
        {
            IRepairable repairable = other.transform.root.gameObject.GetComponent<IRepairable>();

            if (repairable != null) // check if objet can be repaired
            {
                repairableObjects.Add(repairable);
                objectHealth.Add(health);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("structure"))
            RemoveRepairableFromList(other.transform.root.gameObject);
    }

    public void TurretDestroyed(GameObject turret)
    {
        RemoveRepairableFromList(turret);
    }

    private void RemoveRepairableFromList(GameObject GO)
    {
        Health health = GO.GetComponent<Health>();
        if (health != null && objectHealth.Contains(health))
        {
            objectHealth.Remove(health);
        }

        IRepairable repairable = GO.GetComponent<IRepairable>();
        if (repairable != null && repairableObjects.Contains(repairable))
        {
            repairableObjects.Remove(repairable);

            if (repairable == objectToRepair)
            {
                objectToRepair = null;
                FindClosestRepairableObject();
            }
        }
    }

    private void FindClosestRepairableObject()
    {
        float shortestDistance = Mathf.Infinity;

        for (int i = 0; i <objectHealth.Count; i++)
        {
            if (objectHealth[i].currentHealth != objectHealth[i].maxHealth)
            {
                float distance = Vector3.Distance(transform.position, objectHealth[i].gameObject.transform.position);

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    Vector3 objectivePosition = new Vector3(objectHealth[i].gameObject.transform.position.x, transform.position.y, objectHealth[i].gameObject.transform.position.z);
                    objective = objectivePosition;
                    rotateToObject = objectHealth[i].gameObject.transform.position;
                    objectToRepair = repairableObjects[i];
                }
            }
        }
        if (shortestDistance == Mathf.Infinity)
        {
            if (repairing)
            {
                objective = transform.position;
                objectToRepair = null;
            }
        }
    }

    public void EnableHoldPosition()
    {
        wander = false;
        objective = transform.position;
    }

    private void SetInitialWanderPosition()
    {
        originalPosition = transform.position;
    }

    public void EnableWander()
    {
        wander = true;
    }

    private void Wander()
    {
        if (!repairing)
        {
            Vector3 randomOffset = Random.insideUnitSphere * 15;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(originalPosition + randomOffset, out hit, 15, NavMesh.GetAreaFromName("Flyable")))
            {
                objective = hit.position;
            }
        }
    }

    private void RepairObject()
    {
        if (!lineRenderer.enabled)
            lineRenderer.enabled = true;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, rotateToObject);

        if (nextFireTime <= 0)
        {
            objectToRepair.Repair(6);
            nextFireTime = fireRate;
        }
        nextFireTime -= Time.deltaTime;
    }

    private bool ReachedObjective()
    {
        return agent.remainingDistance <= agent.stoppingDistance;
    }

    void Update()
    {
        if (!onNavMesh)
        {
            DoInitialisationMovement();
            return;
        }

        agent.SetDestination(objective);

        if (wander)
        {
            if (ReachedObjective())
            {
                Wander();
            }
        }

        if (objectToRepair != null && ReachedObjective())
        {
            transform.LookAt(rotateToObject);
            RepairObject();
            repairing = true;
        }
        else
        {
            if (lineRenderer.enabled)
                lineRenderer.enabled = false;

            repairing = false;
        }
    }
}
