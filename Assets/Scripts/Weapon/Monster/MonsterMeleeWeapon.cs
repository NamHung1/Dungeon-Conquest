using System.Collections;
using UnityEngine;

public class MonsterMeleeWeapon : MonoBehaviour
{
    public float attackCooldown = 1.5f;
    public float attackRange = 1f;
    public int damage = 10;
    public LayerMask playerLayer;
    public AudioClip attackSound;

    private Transform attackOrigin;
    private Animator animator;
    private AudioSource audioSource;
    private bool isAttacking = false;

    void Start()
    {
        attackOrigin = transform;
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void Attack()
    {
        if (!isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;

        if (animator != null)
            animator.SetTrigger("slash");

        yield return new WaitForSeconds(0.3f);

        if (attackSound != null)
        {
            float volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            audioSource.PlayOneShot(attackSound);
        }

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackOrigin.position, attackRange, playerLayer);
        foreach (var player in hitPlayers)
        {
            player.GetComponent<PlayerStats>()?.TakeDamage(damage);
        }

        yield return new WaitForSeconds(attackCooldown - 0.3f);

        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        if (attackOrigin == null) attackOrigin = transform;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackOrigin.position, attackRange);
    }
}
