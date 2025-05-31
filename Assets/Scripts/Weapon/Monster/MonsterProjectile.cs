using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterProjectile : MonoBehaviour
{
    public int damage = 10;
    public float lifetime = 5f;
    public LayerMask hitLayer;

    private Vector2 moveTo;
    private float moveSpeed;

    public void SetDirection(Vector2 direct, float speed)
    {
        moveTo = direct;
        moveSpeed = speed;
        float angle = Mathf.Atan2(moveTo.y, moveTo.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & hitLayer) != 0)
        {
            PlayerStats ps = collision.GetComponent<PlayerStats>();
            if (ps != null)
                ps.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
