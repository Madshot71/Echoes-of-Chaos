using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract string InteractionPrompt();
    public abstract void Interact(CharacterBase character);
}
