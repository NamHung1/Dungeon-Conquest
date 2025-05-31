using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRangeWeapon : MonoBehaviour
{
    public GameObject projectile;
    public Transform firePoint;
    public AudioClip fireSound;
    public float projectileSpeed = 8f;

    [Header("Fire Mode")]
    public FireMode fireMode = FireMode.Single;

    [Header("Burst Settings")]
    public float burstDelay = 0.2f;
    public int burstCount = 5;

    [Header("Spread Settings")]
    public int spreadCount = 5;
    public float spreadAngle = 30f;

    private Animator animator;
    private AudioSource audioSource;
    private bool hasAnimator = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        hasAnimator = (animator != null);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void Fire(Vector2 targetPos)
    {
        switch (fireMode)
        {
            case FireMode.Single:
                Shoot(targetPos);
                break;
            case FireMode.Burst:
                ShootBurst(targetPos);
                break;
            case FireMode.Spread:
                ShootSpread(targetPos);
                break;
        }
    }

    public void Shoot(Vector2 targetPos)
    {
        if (hasAnimator)
            animator.SetTrigger("shoot");

        PlayFireSound();

        Vector2 direction = (targetPos - (Vector2)firePoint.position).normalized;
        CreateProjectile(direction);
    }

    public void ShootBurst(Vector2 targetPos)
    {
        StartCoroutine(BurstFire(targetPos));
    }

    IEnumerator BurstFire(Vector2 targetPos)
    {
        Vector2 direction = (targetPos - (Vector2)firePoint.position).normalized;

        for (int i = 0; i < burstCount; i++)
        {
            if (hasAnimator)
                animator.SetTrigger("shoot");

            CreateProjectile(direction);

            yield return new WaitForSeconds(burstDelay);
        }
    }

    public void ShootSpread(Vector2 targetPos)
    {
        if (hasAnimator)
            animator.SetTrigger("shoot");

        Vector2 baseDir = (targetPos - (Vector2)firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        float startAngle = baseAngle - spreadAngle * 0.5f;
        float angleStep = spreadAngle / (spreadCount - 1);

        for (int i = 0; i < spreadCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            CreateProjectile(direction);
        }
    }

    private void CreateProjectile(Vector2 direction)
    {
        GameObject proj = Instantiate(projectile, firePoint.position, Quaternion.identity);
        MonsterProjectile mp = proj.GetComponent<MonsterProjectile>();
        mp.SetDirection(direction, projectileSpeed);
    }

    private void PlayFireSound()
    {
        if (fireSound != null)
        {
            float volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            audioSource.PlayOneShot(fireSound);
        }
    }
}

public enum FireMode
{
    Single,
    Burst,
    Spread
}