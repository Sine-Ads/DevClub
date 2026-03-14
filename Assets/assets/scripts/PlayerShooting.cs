using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Setup")]
    public Transform firePoint;     
    public GameObject bulletPrefab; 

    [Header("Settings")]
    public float fireRate = 0.2f;   // Time between shots

    private float nextFireTime = 0f;
    private GunRecoil _gunRecoil;

    void Start()
    {
        // Try original path first
        Transform lhandTransform = transform.Find("lshould/lhand");

        // If that fails, try with Torso in the path
        if (lhandTransform == null)
            lhandTransform = transform.Find("Torso/lshould/lhand");

        if (lhandTransform == null)
        {
            Debug.LogError("Could not find lhand! Is PlayerShooting on the player root?");
            return;
        }

        _gunRecoil = lhandTransform.GetComponent<GunRecoil>();

        if (_gunRecoil == null)
            Debug.LogError("Found lhand but GunRecoil script is not attached to it!");
        else
            Debug.Log("GunRecoil found successfully!"); // ← confirms it worked
    }


    void Update()
    {
        if (Input.GetButton("Fire1") || Input.GetKey(KeyCode.Z))
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

        // 1. Spawn the bullet at the gun's tip position
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // 2. Get the bullet script
        Bullet bullet = bulletObj.GetComponent<Bullet>();

        // 3. Determine direction based on Player's facing direction
        // We check the Player's X scale to know if they are facing Left or Right
        float direction = Mathf.Sign(transform.localScale.x);

        // 4. Send direction to bullet
        if (bullet != null)
        {
            bullet.SetDirection(direction);
        }

        if (_gunRecoil != null)
            _gunRecoil.TriggerRecoil();
    }
}