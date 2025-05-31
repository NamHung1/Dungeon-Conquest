using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float speed = 5f;
    public float acceleration = 5f;
    public float absorbDistance = 0.4f;
    public int amount = 10;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= absorbDistance)
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.Heal(amount);
            }
            Destroy(gameObject);
        }
        else
        {
            float moveSpeed = speed + (acceleration / Mathf.Max(distance, 0.1f));
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }
}
