using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSpin : BaseSkill
{
    public float spinDuration = 1f;
    public GameObject spinEffectPrefab;

    public float damage = 20f;
    public float hitRadius = 2f;
    public LayerMask enemyLayer;

    public AudioClip spinSound;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    protected override void ExecuteSkill()
    {
        if (spinSound != null && audioSource != null)
        {
            float volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            audioSource.PlayOneShot(spinSound, volume);
        }
        StartCoroutine(SpinCoroutine());
    }


    private IEnumerator SpinCoroutine()
    {
        GameObject effect = Instantiate(spinEffectPrefab, transform.position, Quaternion.identity);
        effect.transform.SetParent(transform);

        Animator anim = effect.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Spin");
        }

        float timer = 0f;
        HashSet<Transform> damagedEnemies = new HashSet<Transform>();

        while (timer < spinDuration)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitRadius, enemyLayer);
            foreach (var hit in hits)
            {
                if (!damagedEnemies.Contains(hit.transform))
                {
                    damagedEnemies.Add(hit.transform);
                    EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage((int)damage);
                    }
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(effect);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}
