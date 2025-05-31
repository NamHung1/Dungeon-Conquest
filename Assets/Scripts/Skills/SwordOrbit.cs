using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordOrbit : BaseSkill
{
    public GameObject swordPrefab;
    public float orbitRadius = 3f;
    public float orbitDuration = 1f;
    public float swordSpeed = 10f;
    public int swordCount = 7;

    public AudioClip summonSound;
    private AudioSource audioSource;

    private List<GameObject> swords = new List<GameObject>();

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
        if (summonSound != null && audioSource != null)
        {
            float volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            audioSource.PlayOneShot(summonSound, volume);
        }
        StartCoroutine(SummonSwords());
    }

    private IEnumerator SummonSwords()
    {
        List<GameObject> swords = new List<GameObject>();

        for (int i = 0; i < swordCount; i++)
        {
            float angle = i * Mathf.PI * 2f / swordCount;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * orbitRadius;
            Vector3 spawnPos = transform.position + offset;

            GameObject sword = Instantiate(swordPrefab, spawnPos, Quaternion.identity);
            sword.GetComponent<Longsword>().StartOrbit(transform, angle, orbitRadius, swordSpeed);
            swords.Add(sword);
        }

        yield return new WaitForSeconds(orbitDuration);

        foreach (GameObject sword in swords)
        {
            if (sword != null)
                sword.GetComponent<Longsword>().Launch();
        }
    }

    private void OnDisable()
    {
        foreach (var sword in swords)
        {
            if (sword != null)
                Destroy(sword);
        }
        swords.Clear();
    }
}
