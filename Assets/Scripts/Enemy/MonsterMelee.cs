using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMelee : MonoBehaviour
{
    public float speed;
    public float atkRange = 1.5f;
    public float atkCD = 2f;
    public float playerDetectRange = 5f;
    public Transform detectionPoint;
    public LayerMask playerLayer;
    public MonsterMeleeWeapon meleeWeapon;

    private float atkCDTimer;
    private int otherDirection = 1;
    private State state;
    private Rigidbody2D rb;
    private Transform player;
    private Animator animator;

    private Vector2 wanderDirection;
    private float wanderTimer;
    public float wanderChangeTime = 3f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ChangeState(State.Idle);
        Wander();
    }

    void Update()
    {
        DetectPlayer();

        if (atkCDTimer > 0)
            atkCDTimer -= Time.deltaTime;

        if (state == State.Chasing)
        {
            Move();
        }
    }

    private void DetectPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);
        player = hits.Length > 0 ? hits[0].transform : null;

        if (player != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist <= atkRange && atkCDTimer <= 0)
            {
                atkCDTimer = atkCD;
                rb.velocity = Vector2.zero;
                meleeWeapon.Attack();
            }
            else if (dist > atkRange)
            {
                ChangeState(State.Chasing);
            }
        }
        else
        {
            ChangeState(State.Chasing);
        }
    }

    void Move()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * speed;

            if ((player.position.x > transform.position.x && otherDirection == -1) ||
                (player.position.x < transform.position.x && otherDirection == 1))
            {
                Flip();
            }
        }
        else
        {
            if (wanderTimer <= 0)
            {
                Wander();
            }

            wanderTimer -= Time.deltaTime;
            rb.velocity = wanderDirection * (speed * 0.5f); 
        }
    }

    void Wander()
    {
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        wanderDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
        wanderTimer = wanderChangeTime;

        if ((wanderDirection.x > 0 && otherDirection == -1) ||
            (wanderDirection.x < 0 && otherDirection == 1))
        {
            Flip();
        }
    }

    void Flip()
    {
        otherDirection *= -1;
        transform.localScale = new Vector3(otherDirection, 1, 1);
    }

    void ChangeState(State newState)
    {
        if (state == newState) return;

        animator.SetBool("isIdle", newState == State.Idle);
        animator.SetBool("isChasing", newState == State.Chasing);

        state = newState;
    }
}

public enum State{
    Idle,
    Chasing
}