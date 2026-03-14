using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int health = 100;

    public GameObject deathEffect;
   

    public void TakeDamage (int damage)
    {
        health -= damage;
        if(health <= 0) 
        {
            Die();
          
        }
    }

    public void Die() 
    {
        Instantiate(deathEffect,transform.position,Quaternion.identity);
        Destroy(gameObject);
        
    }
}
