using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostSpike : MonoBehaviour
{
    public int damage = 6;
    public float lifetime = 1f;

    private void Start()
    {
        Destroy(gameObject, lifetime); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth health = other.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
        else if (other.CompareTag("Projectiles"))
        {
            Destroy(other.gameObject); 
        }
    }
}
