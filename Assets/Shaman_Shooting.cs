using UnityEngine;

public class Shaman_Shooting : MonoBehaviour
{
    [Header("Setup")]
    public Transform firePoint;
    public GameObject bulletPrefab;

    [Header("Input Settings")]
    public KeyCode shootKey = KeyCode.N;

    [Header("Settings")]
    public float fireRate = 0.2f;
    private float nextFireTime = 0f;
    private float cachedDirection = -1f;

    void Update()
    {
        if (Input.GetKey(shootKey))
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void Shoot()
    {
        if (firePoint == null || bulletPrefab == null) return;

        if (transform.localScale.x != 0)
            cachedDirection = -Mathf.Sign(transform.localScale.x);

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bullet = bulletObj.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.SetShooter(GetComponent<Collider2D>());
            bullet.SetDirection(cachedDirection);
        }
    }
}