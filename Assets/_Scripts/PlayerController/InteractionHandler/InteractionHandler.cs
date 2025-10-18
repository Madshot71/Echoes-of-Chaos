using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class InteractionHandler : MonoBehaviour
{
    public LayerMask mask;
    public float radius = 3f;
    [SerializeField] private Vector3 offset = Vector3.zero;
    public Transform cameraTransform => Camera.main.transform;
    private PlayerController controller;


    private void OnValidate() {
        controller ??= GetComponent<PlayerController>();    
    }

    public Interactable GetInteractable()
    {
        List<Collider> colliders = Physics.OverlapSphere(transform.TransformPoint(offset), radius, mask).ToList();

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        Interactable interactable = null;
        if (Physics.Raycast(ray, out RaycastHit hit, radius, mask))
        {
            interactable = colliders.Closest<Collider>(hit.point, i =>
            {
                if (i.gameObject.tag == "Interactable")
                {
                    return i.transform.position;
                }
                return Vector3.zero;

            }).GetComponent<Interactable>();
        }

        return interactable;
    }
}
