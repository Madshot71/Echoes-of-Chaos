using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool canInteract { get; protected set; } = true;
    public abstract string InteractionPrompt();
    public abstract void Interact(CharacterBase character);
    public abstract void StopInteract(CharacterBase character);
}
