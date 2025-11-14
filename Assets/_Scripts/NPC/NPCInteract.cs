using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPC))]
public class NPCInteract : Interactable 
{
    private NPC npc;

    Dictionary<string, Action> pairs = new Dictionary<string, Action>();

    private void OnValidate() 
    {
        npc ??= GetComponent<NPC>();    
    }

    private void Awake()
    {
        pairs.Clear();
        pairs.Add("Follow", FollowMe);
        pairs.Add("Hide", Hide);
    }
    
    public override string InteractionPrompt()
    {
        return $"talk to {npc.name}";
    }

    public override void Interact(CharacterBase character)
    {
        if (!canInteract) return;
        canInteract = false;
        DialogManager.instance.ShowOptions(pairs , StopInteract);
    }

    public override void StopInteract(CharacterBase character)
    {
        StopInteract();
    }

    private void StopInteract()
    {
        canInteract = true;
    }

    private void FollowMe()
    {
        Debug.Log("I Will Follow you");
    }
    
    private void Hide()
    {
        Debug.Log("I Will go hide");
    }
}