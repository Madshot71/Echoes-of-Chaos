using UnityEngine;
using UnityEngine.Events;

public class HitBox : MonoBehaviour
{
    [SerializeField] private h_Detectors[] Detedtors = new h_Detectors[15];
     
    public float currentHealth;
    public float maxHealth = 100f; 

    public UnityEvent OnDeath;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (!isDead && currentHealth <= 0)
        {
            isDead = true;
            OnDeath?.Invoke();
        }
    }

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