using UnityEngine;

public class Damage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int health = 100;
    public GameObject deatheffect;

   public void TakeDamage (int damage)
   {
        health -= damage;
        Debug.Log("Player took " + damage + " damage! Health remaining: " + health);

        if (health <= 0)
        {
            Debug.Log("PLAYER DIED!");
            Die();
        }
   }

    void Die() 
    {
        Debug.Log("Die() function called - destroying player");
        if (deatheffect != null)
        {
            Instantiate(deatheffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

}
