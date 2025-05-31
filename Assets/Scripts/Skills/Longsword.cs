using UnityEngine;

public class Longsword : MonoBehaviour
{
    private Transform center;
    private float angle;
    private float radius;
    private float orbitSpeed;
    private bool isOrbiting = false;

    private float launchSpeed = 10f;
    private Vector3 launchDirection;
    private bool isLaunched = false;
    private float lifetime = 3f;

    public void StartOrbit(Transform orbitCenter, float startAngle, float orbitRadius, float speed)
    {
        center = orbitCenter;
        angle = startAngle;
        radius = orbitRadius;
        orbitSpeed = speed;
        isOrbiting = true;
    }

    void Update()
    {
        if (isOrbiting && center != null)
        {
            angle += Time.deltaTime * orbitSpeed;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            Vector3 newPos = center.position + new Vector3(x, y, 0);
            launchDirection = (newPos - center.position).normalized;

            transform.position = newPos;
            transform.right = launchDirection;
        }
        else if (isLaunched)
        {
            transform.position += launchDirection * launchSpeed * Time.deltaTime;
        }
    }

    public void Launch()
    {
        isOrbiting = false;
        isLaunched = true;

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var health = other.GetComponent<EnemyHealth>();
            if (health != null) health.TakeDamage(6);
        }
        else if (other.CompareTag("Projectiles"))
        {
            Destroy(other.gameObject);
        }
    }
}
