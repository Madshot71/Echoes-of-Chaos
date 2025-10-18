using UnityEngine;

public class HitBox : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth = 100f;


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
    }

    public bool Alive()
    {
        return currentHealth > 0;
    }
}