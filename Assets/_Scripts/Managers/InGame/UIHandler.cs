using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
    [Require][SerializeField] private CharacterBase character;
    //Bars
    [SerializeField] private Image health, health_f, stamina, stamin_f;

    //Interaction
    [SerializeField] private TMP_Text interactTxt; 

    [SerializeField] private float lerpSpeed;
    


    public void LateUpdate()
    {
        UI();
        Interact();
    }

    public void UI()
    {
        float h = character.controller.hitBox.currentHealth;
        float h_max = character.controller.hitBox.maxHealth;

        Bars(health, health_f, h.DivideBy(h_max));
        

        if (character.controller.staminaHandler == null)
        {
            return;
        }

        float s = character.controller.staminaHandler.current;
        float s_max = character.controller.staminaHandler.MaxStamina;

        Bars(stamina, stamin_f, s.DivideBy(s_max)); 
    }


    private void Bars(Image bar, Image fade, float value)
    {
        if (bar || fade)
        {
            bar.fillAmount = Mathf.Lerp(bar.fillAmount, value, lerpSpeed * Time.deltaTime);
            fade.fillAmount = Mathf.Lerp(fade.fillAmount, bar.fillAmount, lerpSpeed * Time.deltaTime);
        }
    }
    
    private void Interact()
    {
        if (character.interactable == null)
        {
            interactTxt.text = string.Empty;
            return;
        }

        interactTxt.text = character.interactable.InteractionPrompt();
    }
}