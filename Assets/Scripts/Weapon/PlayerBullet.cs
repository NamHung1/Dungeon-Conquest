using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public int damage = 20; 
    public float timeDestroy = 2f; 
    public float moveSpeed = 25f;
    public LayerMask enemyLayer;

    void Start()
    {
        Destroy(gameObject, timeDestroy);
    }

    void Update()
    {
        BulletMove();
    }

    void BulletMove()
    {
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        //else if (!collision.isTrigger)
        //{
        //    Destroy(gameObject);
        //}
    }
}
