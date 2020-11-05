using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePoint : MonoBehaviour
{
    public List<Enemy_Base> enemies;
    public Health health;

    [SerializeField] private GameObject[] crystals;
    private int crystalsLost;
    private float deactivateThreshold;
    private float remainder;
    private ResourceManager resManager;

    void Start()
    {
        resManager = ResourceManager.instance;
        health = GetComponent<Health>();
        enemies = new List<Enemy_Base>();
        remainder = (float)health.currentHealth / crystals.Length;
        deactivateThreshold = remainder;
        crystalsLost = 0;
    }

    private void OnDisable()
    {
        foreach (Enemy_Base enemy in enemies)
            Enemy_Base.DeathEvent -= EnemyDied;
    }

    public void AddEnemyToList(Enemy_Base enemy)
    {
        Enemy_Base.DeathEvent += EnemyDied;
        enemies.Add(enemy);
    }

    public void RemoveEnemyFromList(Enemy_Base enemy)
    {
        Enemy_Base.DeathEvent -= EnemyDied;
        enemies.Remove(enemy);
    }

    public void EnemyDied(Enemy_Base enemy) // for delegate event remove enemy from list when died
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            Enemy_Base.DeathEvent -= EnemyDied;
        }
    }

    public void TakeResource(int amount)
    {
        if (health.currentHealth > 0 && enemies.Count > 0) // make sure there is an enemy inside the resource trigger before it can take resources (this is if an enemy trys to take resoruces from another point)
        {
            health.currentHealth -= amount;
            resManager.ResourceTaken(amount);

            if (health.currentHealth < 0)
                health.currentHealth = 0;

            int healthLost = health.maxHealth - health.currentHealth;

            if (healthLost >= deactivateThreshold)
            {
                crystals[crystalsLost].SetActive(false);
                crystalsLost++;
                deactivateThreshold += remainder;
            }
        }
    }
}
