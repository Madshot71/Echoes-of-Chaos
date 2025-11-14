using UnityEngine;


public class StaminaHandler : MonoBehaviour
{
    public float Delay = 5f;
    [SerializeField] public float current { get; private set; }
    [field: SerializeField] public float MaxStamina { get; private set; } = 100;
    [field : SerializeField] public float ReplanishRate { get; private set; }

    public bool canuse;
    private float _time;


    private void Awake()
    {
        current = MaxStamina;
    }

    private void Update()
    {
        ReplanishStamina();

        if (canuse == false && _time < Delay)
        {
            _time += Time.deltaTime;
            return;
        }

        if (current <= 0)
        {
            canuse = false;
            return;
        }

        canuse = true;
        _time = 0;
    }

    private void ReplanishStamina()
    {
        current += ReplanishRate * Time.deltaTime;
        current = Mathf.Clamp(current, 0, MaxStamina);
    }


    public void RegenerateStamina(float amount)
    {
        current += amount * Time.deltaTime;
    }

    public void ConsumeStamina(float amount)
    {
        current -= amount * Time.deltaTime;
    }
}