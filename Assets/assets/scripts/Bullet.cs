using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    public int damage = 100;
    public Rigidbody2D rb;

    [Header("Dynamic Movement")]
    public float wobbleAmplitude = 0.08f;
    public float wobbleFrequency = 8f;
    public float launchBoost = 6f;
    public float boostDecay = 5f;
    public bool rotateTravelDirection = true;

    private float direction;
    private float spawnTime;
    private float currentBoost;
    private Collider2D shooterCollider;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Debug.Log("Bullet spawned. RB is null? " + (rb == null));
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
        spawnTime = Time.time;
        currentBoost = launchBoost;

        StartCoroutine(SpawnSquash());
    }

    IEnumerator SpawnSquash()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 squashedScale = new Vector3(originalScale.x * 1.4f, originalScale.y * 0.6f, originalScale.z);

        transform.localScale = squashedScale;

        float elapsed = 0f;
        float duration = 0.12f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(squashedScale, originalScale, elapsed / duration);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        float elapsed = Time.time - spawnTime;

        currentBoost = Mathf.Lerp(currentBoost, 0f, boostDecay * Time.fixedDeltaTime);

        float horizontalSpeed = direction * (speed + currentBoost);
        float wobbleY = Mathf.Sin(elapsed * wobbleFrequency) * wobbleAmplitude;

        rb.linearVelocity = new Vector2(horizontalSpeed, wobbleY * speed);

        if (rotateTravelDirection && rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(0, 0, angle),
                Time.fixedDeltaTime * 20f
            );
        }
    }

    public void SetShooter(Collider2D col)
    {
        shooterCollider = col;
    }

    public void SetDirection(float dir)
    {
        direction = dir;

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction * (speed + launchBoost), 0);
            Debug.Log("Velocity set to: " + rb.linearVelocity);
        }
        else
        {
            Debug.LogError("RB is NULL in SetDirection!");
        }

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Sign(direction) * Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player")) return;
        if (hitInfo == shooterCollider) return;

        Debug.Log("Hit: " + hitInfo.name);

        Damage damageable = hitInfo.GetComponent<Damage>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}