using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public GameObject energy;
    public GameObject health;
    [Range(0f, 1f)] public float energyDropChance = 0.6f;
    [Range(0f, 1f)] public float healthDropChance = 0.2f;
    public int currentHealth;
    public int maxHealth;

    private MonsterSpawner spawner;

    public event Action<GameObject> OnEnemyDied;

    private void Start()
    {
        currentHealth = maxHealth;
        spawner = FindObjectOfType<MonsterSpawner>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Remaining health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died!");

        OnEnemyDied?.Invoke(gameObject);

        DropEnergy();
        DropHealth();

        Destroy(gameObject);
    }

    void DropEnergy()
    {
        if (energy != null && UnityEngine.Random.value <= energyDropChance)
        {
            Instantiate(energy, transform.position, Quaternion.identity);
        }
    }

    void DropHealth()
    {
        if (health != null && UnityEngine.Random.value <= healthDropChance)
        {
            Instantiate(health, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        }
    }
}
