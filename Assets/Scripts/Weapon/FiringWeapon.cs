using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringWeapon : MonoBehaviour
{
    public GameObject bulletPrefab; 
    public Transform firePos; 
    public AudioClip fireSfx;
    public float bulletForce = 20f; 
    public float delay = 0.5f;

    private float nextFireTime = 0.5f;
    private PlayerMovement player;
    private AudioSource audioSource;

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        RotateWeapon();
        Shoot();
    }

    void RotateWeapon()
    {
        if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)
            return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        bool isFacingLeft = player.otherDirection == -1;
        transform.localScale = new Vector3(isFacingLeft ? -1 : 1, 1, 1);
    }

    void Shoot()
    {
        if (Input.GetMouseButtonDown(0) && Time.time > nextFireTime)
        {
            nextFireTime = Time.time + delay;
            Instantiate(bulletPrefab, firePos.position, firePos.rotation);
            FireSound();
        }
    }

    void FireSound()
    {
        if (fireSfx != null && audioSource != null)
        {
            float volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            audioSource.PlayOneShot(fireSfx);
        }
    }
}
