using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int currentHealth;
    public int initialMaxHealth;
    public int maxHealth;

    void Awake()
    {
        initialMaxHealth = maxHealth;
        currentHealth = maxHealth;
    }
}
