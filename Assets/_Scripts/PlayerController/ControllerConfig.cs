using UnityEngine;

[CreateAssetMenu(fileName = "ControllerConfig", menuName = "Player/ControllerConfig", order = 0)]
public class ControllerConfig : ScriptableObject
{
    [Header("Movement")]
    public float movementSpeed = 5f;
    public float rotationSpeed = 10f;
    public float crouchSpeed = 2.5f;
    public float sprintSpeed = 8f;
    public float proneSpeed = 1.5f;
    public float fallSpeed = 10f;
    public float antiBump = -1f;
    public float turnSmoothing = 2f;
    public float maxTurnRate = 5;

    
    [Header("Jumping")]
    public float jumpForceUp = 5f;
    public float jumpForceForward = 2f;

    [Header("ColliderDefault")]
    public float defaultHeight = 2f;
    public float defaultColliderCenterY = 1f;

    [Header("Crouching")]
    public float crouchHeight = 1f;
    public float crouchColliderCenterY = 0.5f;

    [Header("Prone")]
    public float proneHeight = 0.5f;
    public float proneColliderCenterY = 0.25f;
    public float maxProneSlope = 10;

    [Header("Slide")]
    public float slideForce = 5f;
    public float minSlideAngle = 5f;
    public float maxSlideAngle = 45f;


    [field: SerializeField] public float sprintStaminaCost { get; private set;} = 10f;
    [field : SerializeField] public float jumpStaminaCost { get; private set;} = 15f;
    [field: SerializeField] public float slideStaminaCost { get; private set; } = 20f;    


    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float distance = 0.3f;
    public float offset = 0.2f;
    public float radius = 0.2f;
    public float fallDamageTime = 4f;

    [Header("Animations")]
    public float animationSmoothSpeed = 3f;
    public string jumpAnimation = "Jump";
    public string sprintJumpAnimation = "SprintJump";
    public string slideAnimation = "Slide";
    public string landAnimation = "Land";
    public string fallAnimation = "Fall";

}