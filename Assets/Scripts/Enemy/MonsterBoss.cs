using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class MonsterBoss : MonoBehaviour
{
    public float speed;
    public float meleeRange = 2f;
    public float rangeAttackRange = 6f;
    public float atkCD = 3f;
    public float playerDetectRange = 8f;
    public float wanderChangeTime = 3f;
    public Transform detectionPoint;
    public LayerMask playerLayer;
    public BossWeapon bossWeapon;

    private int otherDirection = 1;
    private float atkCDTimer;
    private float wanderTimer;
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

        RotateWeapon();

        if (state == State.Chasing)
            Move();
    }

    void DetectPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);
        player = hits.Length > 0 ? hits[0].transform : null;

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.activeInHierarchy)
            {
                player = hit.transform;
                break;
            }
        }

        if (player != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (atkCDTimer <= 0)
            {
                atkCDTimer = atkCD;

                rb.velocity = Vector2.zero;

                if (dist <= meleeRange)
                    bossWeapon.MeleeAttack();
                else if (dist <= rangeAttackRange)
                    bossWeapon.RangeAttack(player.position);
            }
            else if (dist > meleeRange)
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

    void RotateWeapon()
    {
        if (player != null && player.gameObject.activeInHierarchy)
        {
            Vector2 dirToPlayer = (player.position - bossWeapon.firePoint.position).normalized;
            float angle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;

            if (otherDirection == -1)
                angle += 180f;

            bossWeapon.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            bossWeapon.transform.rotation = Quaternion.identity;
        }
    }

    void ChangeState(State newState)
    {
        if (state == newState) return;

        animator.SetBool("isIdle", newState == State.Idle);
        animator.SetBool("isChasing", newState == State.Chasing);

        state = newState;
    }
}
