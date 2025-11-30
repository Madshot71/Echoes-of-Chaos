using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private CharacterBase main;
    [SerializeField] private BarSlot mainsBars;
    [SerializeField] private List<BarSlot> bars = new();

    [Header("CrossHair")]
    [SerializeField] private Image crossHair;
    [SerializeField] private Image reloadImage;

    [SerializeField] private TMP_Text interactTxt; 
    [field : SerializeField] public float lerpSpeed {get; private set;}
    
    private void OnValidate()
    {
        foreach (var item in bars)
        {
            if(item != null)
                item.Init(this);
        }
        mainsBars?.Init(this);
    }

    public void LateUpdate()
    {
        if(main == null)
        {
            SetInteractTxt(string.Empty);
            return;
        }
        Interact(main.interactable);

        SetReload(main.data);

        //Update All bars
        mainsBars.UI(main.data);
        UpdteBars();
    }
    
    private void UpdteBars()
    {
        for (int i = 0; i < main.Allies.Count; i++)
        {
            bars[i].UI(main.Allies[i].data);
        }
    }
    
    private void Interact(Interactable interactable)
    {
        if (interactable == null){
            SetInteractTxt(string.Empty);
            return;
        }
        SetInteractTxt(interactable.InteractionPrompt());
    }

    private void SetInteractTxt(string value)
    {
        if(interactTxt == null)
        {
            return;
        }
        interactTxt.text = value;
    }

    private void SetReload(CharacterBase.Data data)
    {
        if(crossHair == null || reloadImage == null)
        {
            return;
        }

        if(data.reloadProgress > 0)
        {
            crossHair.enabled = false;
            reloadImage.enabled = true;
            reloadImage.fillAmount = data.reloadProgress;
            return;
        }
        crossHair.enabled = true;
        reloadImage.enabled = false;
    }
}

