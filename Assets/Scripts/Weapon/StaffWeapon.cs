using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffWeapon : MonoBehaviour
{
    public GameObject lightningLinePrefab;
    public float maxJumpDistance = 5f;
    public int maxJumps = 4;
    public int baseDamage = 8;
    public LayerMask enemyLayer;
    public AudioClip castSfx;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CastLightning();
        }
    }

    void CastLightning()
    {
        LightningSound();

        Collider2D[] enemiesInSight = Physics2D.OverlapCircleAll(transform.position, 20f, enemyLayer);
        if (enemiesInSight.Length == 0) return;

        Collider2D firstTarget = GetClosestEnemy(enemiesInSight, transform.position);
        if (firstTarget == null) return;

        List<Collider2D> hitEnemies = new List<Collider2D>();
        hitEnemies.Add(firstTarget);

        DealDamage(firstTarget.transform.position, firstTarget, baseDamage);

        Collider2D current = firstTarget;

        for (int i = 0; i < maxJumps; i++)
        {
            Collider2D nextTarget = GetClosestEnemy(
                Physics2D.OverlapCircleAll(current.transform.position, maxJumpDistance, enemyLayer),
                current.transform.position, hitEnemies);

            if (nextTarget != null)
            {
                hitEnemies.Add(nextTarget);
                DealDamage(current.transform.position, nextTarget, baseDamage);
                current = nextTarget;
            }
            else
            {
                DealDamage(current.transform.position, firstTarget, baseDamage * 2);
                break;
            }
        }
    }

    void LightningSound()
    {
        if (castSfx != null && audioSource != null)
        {
            float volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            audioSource.PlayOneShot(castSfx);
        }
    }

    Collider2D GetClosestEnemy(Collider2D[] enemies, Vector3 fromPosition, List<Collider2D> excludeList = null)
    {
        float minDist = float.MaxValue;
        Collider2D closest = null;

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.isTrigger || (excludeList != null && excludeList.Contains(enemy))) continue;

            float dist = Vector2.Distance(fromPosition, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }

        return closest;
    }

    void DealDamage(Vector3 fromPosition, Collider2D enemy, int dmg)
    {
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(dmg);
        }

        // Create lightning effect
        if (lightningLinePrefab != null)
        {
            GameObject bolt = Instantiate(lightningLinePrefab);
            LineController lc = bolt.GetComponent<LineController>();
            if (lc != null)
            {
                lc.AssignTarget(fromPosition, enemy.transform);
            }

            Destroy(bolt, 0.3f);
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 5f); // Lightning initial range
    }
}
