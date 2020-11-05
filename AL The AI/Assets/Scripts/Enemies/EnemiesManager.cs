using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager instance;
    public List<Enemy_Base> enemies;

    private List<Enemy_Base> deadEnemies;
    private ResourceManager resManager;
    private int rayCastLayer;

    private void Awake()
    {
        instance = this;
        enemies = new List<Enemy_Base>();
    }

    private void Start()
    {
        //enemies = new List<Enemy_Base>();
        deadEnemies = new List<Enemy_Base>();
        resManager = ResourceManager.instance;

        rayCastLayer = 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Interactable");

        InvokeRepeating("ChooseObjective", 0f, 0.5f);
    }

    private void Update()
    {
        if (enemies.Count.Equals(0)) // no enemies to perform this on.
            return;

        //remove any enemies that were marked as dead in the  from the list.
        if (!deadEnemies.Count.Equals(0))
        {
            foreach (Enemy_Base deadEnemy in deadEnemies)
            {
                if (enemies.Contains(deadEnemy))
                {
                    enemies.Remove(deadEnemy); // safe to do as the list of enemies is not being iterated over at this point.
                }
            }
            deadEnemies.Clear();
        }

        foreach (Enemy_Base enemy in enemies)
        {
            if (!enemy.isDead)
            {
                enemy.anim.SetFloat("distance", Vector3.Distance(enemy.objective, enemy.gameObject.transform.position));
                if (enemy.agent.enabled) // change between resource and objective if attacking
                    enemy.agent.SetDestination(enemy.objective);
            }
            else
            {
                deadEnemies.Add(enemy); // mark it to be removed from the list next update.
            }
        }
    }

    private void ChooseObjective()
    {
        if (enemies.Count.Equals(0))
            return;

        SetGatherers();
        SetGuards();

        foreach (Enemy_Base enemy in enemies)
        {
            enemy.target = enemy.targetResource;
            enemy.objective = enemy.closestResource;
            enemy.anim.SetBool("targetIsStructure", false);

            if (enemy.resPoint != null) 
            {
                enemy.anim.SetBool("canWalk", true);
                enemy.anim.SetBool("canGather", true);
                enemy.anim.SetBool("canGuard", false);
            }
            else 
            {
                enemy.anim.SetBool("canWalk", true);
                enemy.anim.SetBool("canGather", false);
                enemy.anim.SetBool("canGuard", true);
            }

            if (!enemy.objectsInRange.Count.Equals(0)) 
            {
                bool canSeeObject = false;

                foreach (GameObject obj in enemy.objectsInRange) 
                {
                    if (CanSeeStructure(enemy.gameObject.transform.position, obj.transform.position))
                        canSeeObject = true;
                }

                if (canSeeObject)
                {
                    enemy.target = enemy.targetStructure;
                    enemy.anim.SetBool("targetIsStructure", true);
                    enemy.anim.SetBool("canWalk", true);
                    enemy.anim.SetBool("canGather", false);
                    ChooseClosestStructure(enemy); 
                }
            }
        }
    }

    private bool CanSeeStructure(Vector3 enemyPosition, Vector3 structure) // check view to structure is not obstructed.
    {
        if (Physics.Raycast(enemyPosition + Vector3.up, ((structure + Vector3.up) - (enemyPosition + Vector3.up)), out RaycastHit Hit, Mathf.Infinity, rayCastLayer, QueryTriggerInteraction.Ignore))
        {
            if (Hit.transform.root.gameObject.CompareTag("structure"))
            {
                return true;
            }
        }
        return false;
    }

    private void ChooseClosestStructure(Enemy_Base enemy)
    {
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject obj in enemy.objectsInRange)
        {
            float distance = Vector3.Distance(enemy.gameObject.transform.position, obj.transform.position);

            if (distance < shortestDistance && CanSeeStructure(enemy.gameObject.transform.position, obj.transform.position)) // check if this is closer than the last object and if we can see it.
            {
                shortestDistance = distance;
                enemy.objective = obj.transform.position;

                IDamageable damageable = obj.GetComponent<IDamageable>();

                if (damageable != null)
                    enemy.damageableStructure = damageable;
            }
        }
        if (shortestDistance != Mathf.Infinity)
        {
            if (CanSeeStructure(enemy.gameObject.transform.position, enemy.objective)) // can we still see the structure to shoot at it?
                enemy.anim.SetBool("canShoot", true);
        }
        else
        {
            enemy.anim.SetBool("canShoot", false);
            ChooseObjective(); // might not be needed
        }
    }

    private void SetGatherers()
    {
        foreach (ResourcePoint resource in resManager.resourcePoints) // iterate through resource points.
        {
            if (resource.health.currentHealth > 0) // check if the resource point still has something to gather.
            {
                List<Enemy_Base> closestResourcePoint_Enemies = new List<Enemy_Base>(); // a list to hold up to 5 closest enemies found to a resource point. used for comparing current assigned enemies of a resource point.

                for (int i = 0; i < 5; i++) // do the following n amount of times so that we can fill the resource to max capacity with the closest gatherers.
                {
                    Enemy_Base closestEnemy = null; // used to store a reference to the closest enemy as we search through the list.

                    float shortestDistance = Mathf.Infinity;

                    foreach (Enemy_Base enemy in enemies) // iterate through all enemies.
                    {
                        float distance = Vector3.Distance(enemy.gameObject.transform.position, resource.gameObject.transform.position); // distance between enemy and resource.

                        if (distance < shortestDistance) // check if the enemy distance is less than the previously checked enemy distance.
                        {
                            if (closestResourcePoint_Enemies.Contains(enemy)) // already added to closest enemies list this cycle.
                            {
                                continue; // skip this one.
                            }
                            else if (enemy.resPoint != null && enemy.resPoint.health.currentHealth > 0 && !resource.enemies.Contains(enemy)) // if the enemy has been previously assigned to another resource and it still has something to gather, check if this resource is closer than their previous assigned one.
                            {
                                float previousClosestDistance = Vector3.Distance(enemy.gameObject.transform.position, enemy.closestResource); // distance to their current assigned resource point.

                                if (distance > previousClosestDistance) // if the distance to this resource point is more than my current assigned resource then skip me. otherwise include me to potentially assign this resource point.
                                {
                                    continue; // go to the next iteration. else include me.
                                }
                            }
                            shortestDistance = distance; // assign a new shortest distance.
                            closestEnemy = enemy; // assing to be the current closest enemy.
                        }
                    }
                    if (closestEnemy != null) // a closest enemy was found.
                    {
                        if (closestEnemy.resPoint != null && !resource.enemies.Contains(closestEnemy))
                        {
                            closestEnemy.resPoint.RemoveEnemyFromList(closestEnemy);
                        }
                        closestResourcePoint_Enemies.Add(closestEnemy); // add them to the list of closest enemies
                    }
                }

                foreach (Enemy_Base oldEnemy in resource.enemies) // remove old enemies that are not still the closest. these will be assigned to another crystal on next loop OR set to guard closest crystal.
                {
                    oldEnemy.resPoint = null;
                }
                resource.enemies.Clear(); // empty out the resources list of gatherers to start fresh.

                foreach (Enemy_Base enemy in closestResourcePoint_Enemies) // assign this resource point to each of the enemies we found.
                {
                    resource.AddEnemyToList(enemy);

                    // set their references to this resource
                    enemy.closestResource = resource.gameObject.transform.position;
                    enemy.resPoint = resource;
                }
            }
        }
    }

    private void SetGuards()
    {
        // enemies that were not assigned a crystal to gather should be given the position of their closest crystal to 'guard' it. also if a resource has since emptied they should guard next.
        int index = 0;
        foreach (Enemy_Base enemy in enemies)
        {
            if (enemy.resPoint == null || enemy.resPoint.health.currentHealth <= 0)
            {
                enemy.resPoint = null;

                float shortestDistance = Mathf.Infinity;
                Vector3 closestResource = Vector3.zero;

                foreach (ResourcePoint resource in resManager.resourcePoints)
                {
                    if (resource.health.currentHealth > 0) // only check against acitive resources
                    {
                        float distance = Vector3.Distance(enemy.gameObject.transform.position, resource.gameObject.transform.position); // distance between enemy and resource.

                        if (distance < shortestDistance)
                        {
                            shortestDistance = distance;
                            closestResource = resource.gameObject.transform.position;
                        }
                    }
                }
                if (closestResource != Vector3.zero)
                {
                    List<Vector3> positionList = GetPositionsAroundResource(closestResource, 7f, 15);

                    enemy.closestResource = positionList[index];
                    index = (index + 1) % positionList.Count; // ramainer to ensure not index out of bounds.
                }
            }
        }
    }

    private List<Vector3> GetPositionsAroundResource(Vector3 startingPosition, float radius, int positions)
    {
        List<Vector3> positionList = new List<Vector3>();

        for(int i = 0; i < positions; i++)
        {
            int angle = i * (360 / positions); // define an angle radius around 360 degrees
            Vector3 newPosition = startingPosition + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius; // starting position plus direction * radius, to form the circular positions

            positionList.Add(newPosition);
        }

        return positionList;
    }
}
