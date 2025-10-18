using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class StaminaHandler : MonoBehaviour
{
    public float current { get; private set; }
    public float MaxStamina { get; private set; }
    public float ReplanishRate { get; private set; }

    private PlayerController controller;

    private void OnValidate() 
    {
        controller ??= GetComponent<PlayerController>();
    }

    private void Update()
    {
        ReplanishStamina();
    }

    private void ReplanishStamina()
    {
        current += ReplanishRate * Time.deltaTime;
        current = Mathf.Clamp(current, 0, MaxStamina);
    }


    private void RegenerateStamina(float amount)
    {
        current += amount;
    }

    private void ConsumeStamina(float amount)
    {
        current -= amount;
    }
}