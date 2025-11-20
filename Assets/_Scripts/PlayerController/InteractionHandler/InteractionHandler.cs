using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(PlayerController))]
public class InteractionHandler : MonoBehaviour
{
    public LayerMask mask;
    public float radius = 3f;
    public Transform cameraTransform => Camera.main.transform;
    private PlayerController controller;

    public bool isInteracting { get; private set; } = false;
    private Collider[] hits;

    private void OnValidate() {
        controller ??= GetComponent<PlayerController>();    
    }

    public Interactable GetInteractable()
    {
        if(controller.hitBox.Alive() == false)
        {
            return null;
        }

        hits = Physics.OverlapSphere(transform.position , radius ,mask);
        Interactable interactable = null;

        for (int i = 0; i < hits.Count(); i++)
        {
            if (!hits[i].transform.TryGetComponent<Interactable>(out Interactable component))
            {
                continue;
            }

            if(component.canInteract == false)
            {
                continue;
            }

            if (interactable == null)
            {
                interactable = component;
                continue;
            }

            if (Vector3.Distance(component.transform.position, transform.position) <
                Vector3.Distance(interactable.transform.position, transform.position))
            {
                interactable = component;
            }
        }

        return interactable;

    }
}
