using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRange : MonoBehaviour
{
    public float speed;
    public float atkRange = 5f;
    public float atkCD = 2f;
    public float playerDetectRange = 7f;
    public float wanderChangeTime = 3f;
    public LayerMask playerLayer;
    public Transform detectionPoint;
    public MonsterRangeWeapon rangeWeapon;

    private float atkCDTimer;
    private float wanderTimer;
    private int otherDirection = 1;
    private Vector2 wanderDirection;
    private State state;
    private Rigidbody2D rb;
    private Transform player;
    private Animator animator;


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

        if (player != null)
        {
            Vector2 dirToPlayer = (player.position - rangeWeapon.firePoint.position).normalized;
            float angle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;

            if (otherDirection == -1) // Facing left
                angle += 180f;

            rangeWeapon.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            rangeWeapon.transform.rotation = Quaternion.identity;
        }

        if (state == State.Chasing)
            Move();
    }

    void DetectPlayer()
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
                rangeWeapon.Fire(player.position);
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
            Vector2 dir = (player.position - transform.position).normalized;
            rb.velocity = dir * speed;

            if ((player.position.x > transform.position.x && otherDirection == -1) ||
                (player.position.x < transform.position.x && otherDirection == 1))
                Flip();
        }
        else
        {
            if (wanderTimer <= 0)
                Wander();

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
            Flip();
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
