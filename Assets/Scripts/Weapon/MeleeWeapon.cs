using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    private float rotateOffSet = 180f;
    private PlayerMovement player;
    private Animator animator;
    private AudioSource audioSource;

    public Transform atkPoint;
    public AudioClip atkSfx;
    public int damage = 20;
    public float attackRange = 1.5f;
    public LayerMask enemyLayer;
    public float atkPointDistance = 1.0f;

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        RotateWeapon();
        Attack();
    }

    void RotateWeapon()
    {
        if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)
        {
            return;
        }
        Vector3 orient = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(orient.y, orient.x) * Mathf.Rad2Deg;
        float adjustedOffset = player.otherDirection == -1 ? 0f : 180f;

        bool isFacingLeft = player.otherDirection == -1;

        transform.rotation = Quaternion.Euler(0, 0, angle + rotateOffSet);
        transform.localScale = new Vector3(isFacingLeft ? -1 : 1, 1, 1);

        // Update atkPoint position based on weapon rotation
        Vector3 direction = transform.right;
        atkPoint.position = transform.position + direction * atkPointDistance;
    }

    void Attack()
    {
        if(Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
            PlaySound();
            DealDamage();
        }
    }

    void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(atkPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (!enemy.isTrigger)
            {
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                }
            }
        }
    }

    void PlaySound()
    {
        if (atkSfx != null && audioSource != null)
        {
            float volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            audioSource.PlayOneShot(atkSfx);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (atkPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(atkPoint.position, attackRange);
    }
}
