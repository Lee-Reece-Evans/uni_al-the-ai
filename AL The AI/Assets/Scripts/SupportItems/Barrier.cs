using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// very basic blocker, enemies don't take into consideration if it is blocking a path, they just attack if it is in range...
//not enough time to make this better. as well as other issues... instead it's cost is 0, it does nothing ingame but is still usable...
public class Barrier : MonoBehaviour, IHasSibling, IDamageable, IRepairable, IPooledObject
{
    public string poolTag;
    public GameObject siblingGO;
    public Transform[] laserStartPositions;
    public Material laserMat;
    private Transform[] laserEndPositions;
    private Health health;
    public bool isBroken = false;

    private void OnEnable()
    {
        health = GetComponent<Health>(); // initialise health

        if (siblingGO == null)
        {
            return;
        }

        Barrier barrier = siblingGO.GetComponent<Barrier>();

        if (barrier != null) // sibling is the real object
        {
            barrier.health = health; // set sibling to use same health as this object so that repair and damage are shared values
            laserEndPositions = barrier.laserStartPositions; // get end points

            LineRenderer laserBeams = gameObject.AddComponent<LineRenderer>(); // add a line renderer to this;

            // settings for line renderer
            laserBeams.material = laserMat;
            laserBeams.widthMultiplier = 0.025f;
            laserBeams.positionCount = laserStartPositions.Length * 2;

            // loop through start and end point arrays to set line renderer positions.
            int point = 0;
            for (int i = 0; i < laserStartPositions.Length; i++)
            {
                if (IsOdd(i))
                {
                    laserBeams.SetPosition(point, laserEndPositions[i].position);
                    point += 1;
                    laserBeams.SetPosition(point, laserStartPositions[i].position);
                    point += 1;
                }
                else
                {
                    laserBeams.SetPosition(point, laserStartPositions[i].position);
                    point += 1;
                    laserBeams.SetPosition(point, laserEndPositions[i].position);
                    point += 1;
                }
            }
        }
    }

    private void OnDisable()
    {
        if (isBroken)
        {
            isBroken = false;
            siblingGO = null;
            health.currentHealth = health.maxHealth;
        }
    }

    public void PassSiblingGO(GameObject sibling)
    {
        siblingGO = sibling;
    }

    private bool IsOdd(int number)
    {
        return number % 2 != 0;
    }

    public void TakeDamage(int damage, DamageTypes _damageType)
    {
        if (health.currentHealth > 0)
        {
            health.currentHealth -= damage;

            if (health.currentHealth <= 0)
            {
                health.currentHealth = 0;

                if (!isBroken)
                {
                    isBroken = true;

                    // put me back into the pool - sibling first
                    ObjectPool.Instance.ReturnToPool(poolTag, siblingGO);
                    ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
                }
            }
        }
    }

    public void Repair(int amount)
    {
        if (health.currentHealth < health.maxHealth)
        {
            health.currentHealth += amount;

            if (health.currentHealth >= health.maxHealth)
            {
                health.currentHealth = health.maxHealth;
            }
        }
    }

    public void SetPoolDetails(string tag)
    {
        poolTag = tag;
    }
}
