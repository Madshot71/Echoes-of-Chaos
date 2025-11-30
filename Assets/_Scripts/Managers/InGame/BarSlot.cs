using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarSlot : MonoBehaviour
{
    private UIHandler handler;
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private Image health, health_f, stamina, stamin_f;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text currentAmmo , totalAmmo;
    

    public void Init(UIHandler handler)
    {
        this.handler = handler;
    }

    public void UI(CharacterBase.Data data)
    {
        Bars(health, health_f, data.health.DivideBy(data.maxHealth));
        Bars(stamina, stamin_f, data.stamina.DivideBy(data.maxStamina)); 
        SetName(data.name);
    }

    private void SetName(string name)
    {
        if(nameTxt == null)
        {
            return;
        }
        nameTxt.text = name;
    }

    private void SetWeapon()
    {
        
    }

    private void Bars(Image bar, Image fade, float value)
    {
        if (bar && fade)
        {
            bar.fillAmount = Mathf.Lerp(bar.fillAmount, value, handler.lerpSpeed * Time.deltaTime);
            fade.fillAmount = Mathf.Lerp(fade.fillAmount, bar.fillAmount, handler.lerpSpeed * Time.deltaTime);
        }
    }
}