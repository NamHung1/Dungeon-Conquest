using System.Collections;
using UnityEngine;

public class FrostSpikeSkill : BaseSkill
{
    public GameObject frostSpikePrefab;
    public int spikesPerTrail = 6;
    public float spikeSpacing = 0.8f;
    public float angleOffsetDegrees = 15f;
    public AudioClip castSound;
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
        if (castSound != null && audioSource != null)
        {
            float volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            audioSource.PlayOneShot(castSound, volume);
        }
        StartCoroutine(SummonFrostTrails());
    }

    private IEnumerator SummonFrostTrails()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorldPos - transform.position).normalized;
        float baseAngle = Mathf.Atan2(direction.y, direction.x);

        float[] angleOffsets = { 0, angleOffsetDegrees * Mathf.Deg2Rad, -angleOffsetDegrees * Mathf.Deg2Rad };

        foreach (float angleOffset in angleOffsets)
        {
            float angle = baseAngle + angleOffset;
            Vector2 shootDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            StartCoroutine(SummonTrailFrom(transform.position, shootDir));
        }

        yield return null;
    }

    private IEnumerator SummonTrailFrom(Vector3 startPos, Vector2 direction)
    {
        for (int i = 0; i < spikesPerTrail; i++)
        {
            Vector3 spawnPos = startPos + (Vector3)(direction * i * spikeSpacing);
            GameObject spike = Instantiate(frostSpikePrefab, spawnPos, Quaternion.identity);
            spike.transform.right = direction; 
            yield return null;
        }
    }
}
