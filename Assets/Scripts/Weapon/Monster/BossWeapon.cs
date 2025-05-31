using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeapon : MonoBehaviour
{
    public Transform firePoint;
    public GameObject projectile;
    public float projectileSpeed = 10f;
    public int meleeDamage = 20;
    public float meleeRange = 1.5f;
    public LayerMask playerLayer;
    public AudioClip meleeSound;
    public AudioClip rangeSound;

    private Animator animator;
    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void MeleeAttack()
    {
        if (animator != null)
            animator.SetTrigger("slash");

        PlaySound(meleeSound);

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, meleeRange, playerLayer);
        foreach (var player in hitPlayers)
        {
            player.GetComponent<PlayerStats>()?.TakeDamage(meleeDamage);
        }
    }

    public void RangeAttack(Vector2 targetPos)
    {
        if (animator != null)
            animator.SetTrigger("shoot");

        PlaySound(rangeSound);

        Vector2 dir = (targetPos - (Vector2)firePoint.position).normalized;

        // Fire 3 arrows in shape ">"
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float[] angleOffsets = { 0, 15, -15 };

        foreach (float offset in angleOffsets)
        {
            float angle = baseAngle + offset;
            Vector2 newDir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            GameObject proj = Instantiate(projectile, firePoint.position, Quaternion.identity);
            proj.GetComponent<MonsterProjectile>().SetDirection(newDir, projectileSpeed);
        }
    }

    void PlaySound(AudioClip clip)
    {
        float volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}
