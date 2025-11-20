using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(Animator))]
[RequireComponent(typeof(HitBox))]
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [Require][SerializeField] private ControllerConfig config;
    public ControllerCamera camController;

    [Header("Debug")]
    [SerializeField] private float slope;
    [SerializeField] private bool _grounded;
    [SerializeField] private bool isLanded;

    //public 
    public HitBox hitBox { get; private set; }
    public WeaponSystem weaponSystem { get; private set; }
    public StaminaHandler staminaHandler { get; private set; }
    public CharacterBase _characterBase { get; private set; }
    public InteractionHandler interact { get; private set; }

    public InputReceiver receiver;
    public MovementState currentMovementState;


    //Required Components
    public Rigidbody _rigidbody { get; private set; }
    public Animator animator { get; private set; }
    

    //private
    private Vector3 groundNormals;
    private CapsuleCollider _collider;
    public Transform _camera { get; protected set; }


    private float currentTurnRateDegrees;
    private float previousYAngle;
    private float airTime;

    private const string
    CrouchParam = "isCrouching",
    ProneParam = "isProne",
    InteractParam = "isInteracting",
    JumpParam = "isJumping",
    SlideParam = "isSliding",
    TurnParam = "Rotation";


    // For AI use
    

    public enum MovementState
    {
        Idle = 0,
        Walking,
        Sprinting,
        Crouching,
        Prone
    }

    private void OnValidate()
    {
        _rigidbody ??= GetComponent<Rigidbody>();
        _collider ??= GetComponent<CapsuleCollider>();
        animator ??= GetComponent<Animator>();
        hitBox ??= GetComponent<HitBox>();
        interact ??= GetComponent<InteractionHandler>();
        

        //Set Collider
        SetCollider(new Vector3(0, config.defaultColliderCenterY, 0), config.defaultHeight);

        _rigidbody.freezeRotation = true;
    }

    private void Awake()
    {
        TryGetComponent<StaminaHandler>(out StaminaHandler stamina);
        TryGetComponent<WeaponSystem>(out WeaponSystem weapon);

        this.weaponSystem ??= weapon;
        this.staminaHandler ??= stamina;

        currentMovementState = MovementState.Idle;
        SetCameraTransform();
    }

    public virtual void SetCameraTransform()
    {
        _camera = Camera.main.transform;
    }

    private void Update()
    {
        if (hitBox.Alive() == false)
        {
            return;
        }

        HandleFallingAndLanding();

        //Actions
        JumpAction();
        SlideAction();
        CrouchToggle();
        ProneToggle();
    }

    private void FixedUpdate()
    {
        if (hitBox.Alive() == false)
        {
            return;
        }

        //Physics
        Movement();
        Rotate();
    }

    private void LateUpdate()
    {
        Animate();

        if (_characterBase == null) return;

        if (_characterBase.controller != this)
        {
            _characterBase = null;
        }
    }


    public void Init(CharacterBase _base)
    {
        _characterBase = _base;
    }

    private void ResetMovementState()
    {
        if (currentMovementState == MovementState.Crouching || currentMovementState == MovementState.Prone)
        {
            return;
        }
        currentMovementState = MovementState.Idle;
    }

    #region Physics

    private void Movement()
    {
        Vector3 direction = _camera.forward * receiver.movementInput.y + _camera.right * receiver.movementInput.x;
        direction.y = 0;

        direction += Vector3.down * config.antiBump;
        direction.Normalize();

        //check if were sprinting
        float speed = 0;

        if (currentMovementState == MovementState.Crouching)
        {
            speed = config.crouchSpeed;
        }
        else if (currentMovementState == MovementState.Prone)
        {
            speed = config.proneSpeed;
        }
        else if (receiver.movementInput != Vector2.zero)
        {
            speed = config.movementSpeed;
            currentMovementState = MovementState.Walking;
        }
        else
        {
            ResetMovementState();
        }

        if (receiver.sprintInput && canSprint() && receiver.movementInput.y > 0)
        {
            speed = config.sprintSpeed;
            currentMovementState = MovementState.Sprinting;
            staminaHandler?.ConsumeStamina(config.sprintStaminaCost);
        }

        //applying force
        _rigidbody.velocity = direction * speed;
    }

    private void Rotate()
    {
        Vector3 direction = _camera.forward * receiver.movementInput.y + _camera.right * receiver.movementInput.x;
        direction.y = 0;
        direction.Normalize();

        if (direction == Vector3.zero)
        {
            return;
        }

        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        Quaternion rotation = Quaternion.Slerp(transform.rotation, toRotation, config.rotationSpeed * Time.fixedDeltaTime);
        _rigidbody.MoveRotation(rotation);
    }

    
    #endregion

    private void Animate()
    {
        float horizontal = animator.GetFloat("Horizontal");
        float vertical = animator.GetFloat("Vertical");

        Vector2 movement = new Vector2(receiver.movementInput.x, receiver.movementInput.y);
        horizontal = Mathf.Lerp(horizontal, movement.x, config.animationSmoothSpeed * Time.deltaTime);

        if (receiver.sprintInput && canSprint() && movement.y > 0)
        {
            vertical = Mathf.Lerp(vertical, 2, config.animationSmoothSpeed * Time.deltaTime);
        }
        else
        {
            vertical = Mathf.Lerp(vertical, movement.y, config.animationSmoothSpeed * Time.deltaTime);
        }
        
        animator.SetFloat("Horizontal", ClampAbs(horizontal, 0.05f));
        animator.SetFloat("Vertical", ClampAbs(vertical, 0.05f));
    }

    private void HandleFallingAndLanding()
    {
        if (ParamOn(JumpParam))
        {
            _grounded = false;
            slope = 0;
            isLanded = false;
            groundNormals = Vector3.up;
            return;
        }
        
        
        if (isGrounded(out RaycastHit hit))
        {
            if (isLanded == false)
            {
                isLanded = true;
                HandleLanding();
            }

            airTime = 0f;
            slope = Vector3.Angle(hit.normal, Vector3.up);
            groundNormals = hit.normal;
            _grounded = true;
            return;
        }


        airTime += Time.deltaTime;
        slope = 0;
        isLanded = false; 
        groundNormals = Vector3.up;;
        _grounded = false;
        _rigidbody.AddForce(Vector3.down * config.fallSpeed);
        PlayTargetAnimation("Fall", true, false);
    }

    private void HandleLanding()
    {
        if (airTime > config.fallDamageTime)
        {
            //Apply fall Damage
            hitBox.TakeDamage(5 * (airTime - config.fallDamageTime));
        }

        if (currentMovementState == MovementState.Walking)
        {
            //normal landing
            PlayTargetAnimation("forwardLanding", true, false);
        }
        else if (currentMovementState == MovementState.Sprinting)
        {
            //Sprinting 
            if (airTime > 2f)
            {
                PlayTargetAnimation("RollLandin", true, false);
            }
            else
            {
                PlayTargetAnimation("forwardLanding", true, false);
            }
        }
        else
        {
            //idle landing
            PlayTargetAnimation(config.landAnimation, true, false);
        }
    }

    private void HandleTurning()
    {
        // --- Angular Velocity Handling (NEW METHOD) ---
        float currentYAngle = transform.eulerAngles.y;
        // Calculate the change in Y-angle since the last frame
        float deltaAngle = Mathf.DeltaAngle(previousYAngle, currentYAngle);
        // Calculate the instantaneous turn rate in degrees per second
        float instantaneousTurnRate = deltaAngle / Time.fixedDeltaTime;
        // Smooth the turn rate
        currentTurnRateDegrees = Mathf.Lerp(
            currentTurnRateDegrees,
            instantaneousTurnRate,
            config.turnSmoothing * Time.fixedDeltaTime
        );
        // Update the previous Y-angle for the next frame
        previousYAngle = currentYAngle;
        // Normalize for the animator
        float normalizedAngularVelocity = 0f;
        if (Mathf.Abs(config.maxTurnRate) > Mathf.Epsilon) { normalizedAngularVelocity = Mathf.Clamp(currentTurnRateDegrees / config.maxTurnRate, -1f, 1f); }
        animator.SetFloat(TurnParam, normalizedAngularVelocity);   
    }

    #region Actions

    private void JumpAction()
    {
        if (receiver.jumpInput == false)
        {
            return;
        }
        receiver.jumpInput = false;

        if (currentMovementState == MovementState.Crouching || currentMovementState == MovementState.Prone)
        {
            if (CanGetUp())
            {
                currentMovementState = MovementState.Idle;
                ResetToIdleCollider();
            }
            return;
        }

        PlayJumpAnimaiton();
    }
    

    void PlayJumpAnimaiton()
    {
        animator.SetBool(JumpParam , true);
        Debug.Log("Jump");

        if (receiver.sprintInput)
        {
            PlayTargetAnimation(config.sprintJumpAnimation, true, false);
            return;
        }

        PlayTargetAnimation(config.jumpAnimation, true, false);
    }
    public void TriggerJumpPhysics()
    {
        float JumpStaminaMulti = 1;
        switch (currentMovementState)
        {
            case MovementState.Idle:
                _rigidbody.AddForce(transform.up * config.jumpForceUp,
                    ForceMode.Impulse);
                break;

            case MovementState.Sprinting:
                _rigidbody.AddForce(transform.up * config.jumpForceUp + transform.forward * config.jumpForceForward,
                    ForceMode.Impulse);
                JumpStaminaMulti = 1.5f;
                break;

            case MovementState.Walking:
                _rigidbody.AddForce(transform.up * config.jumpForceUp + transform.forward * (config.jumpForceForward / 2),
                    ForceMode.Impulse);
                JumpStaminaMulti = 1.2f;
                break;

            default:
                break;
        }

        staminaHandler?.ConsumeStamina(config.jumpStaminaCost * JumpStaminaMulti);
    }

    private bool SlideCompleted = true;
    private void SlideAction()
    {
        bool isSliding = animator.GetBool(SlideParam);
        bool holdSlide = animator.GetBool("HoldSlide");

        // Only allow holding slide if sliding, grounded, sprinting, not crouching, not jumping, and not using root motion
        if (!_grounded || currentMovementState == MovementState.Crouching ||
            currentMovementState == MovementState.Prone || receiver.crouchInput || ParamOn(JumpParam) || animator.applyRootMotion)
            return;

        //this makes sure slide is only done once if the key is held
        if (isSliding == false && SlideCompleted == false)
        {
            receiver.slideInput = false;
            SlideCompleted = true;
        }

        // If slide input is pressed and not interacting, start slide animation and set up collider
        if (receiver.slideInput && receiver.sprintInput && SlideCompleted == true && !ParamOn(InteractParam) && isSliding == false)
        {
            PlayTargetAnimation(config.slideAnimation, true, false);
            animator.SetBool(SlideParam, true);
            SlideCompleted = false;
            staminaHandler?.ConsumeStamina(config.slideStaminaCost);

            // Force crouch collider state during slide
            SetCollider(new Vector3(0, config.crouchColliderCenterY, 0), config.crouchHeight);
        }

        animator.SetBool("HoldSlide", canHoldSlide() && receiver.sprintInput);

        // Apply sliding force
        if (isSliding && receiver.slideInput)
        {
            // Only apply down slope force if on a slope
            if (slope > config.minSlideAngle && slope < config.maxSlideAngle && holdSlide)
            {
                // Calculate down slope direction
                Vector3 downslopeDir = Vector3.ProjectOnPlane(Vector3.down, groundNormals).normalized;

                // Blend between down slope and player's forward direction (favoring downslope more as slope gets steeper)
                float slopeT = Mathf.InverseLerp(config.minSlideAngle, config.maxSlideAngle, slope);
                Vector3 slideDir = Vector3.Slerp(transform.forward, downslopeDir, slopeT).normalized;

                _rigidbody.AddForce(slideDir * config.slideForce);
            }
            else
            {
                // On flat or shallow slope, slide in player's forward direction
                _rigidbody.AddForce(transform.forward * config.slideForce, ForceMode.Force);
            }
        }

        if (canHoldSlide() == false)
        {
            if (ParamOn(InteractParam) == false)
            {
                // If not sliding, ensure the capsule collider is reset to normal height
                ResetToIdleCollider();
            }
        }
    }

    bool canHoldSlide()
    {
        if (slope < config.maxSlideAngle && slope > config.minSlideAngle)
        {
            if (Vector3.Dot(groundNormals, transform.forward) < 0)
            {
                animator.SetBool(CrouchParam , false);
                animator.SetBool(ProneParam , false);
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    private void CrouchToggle()
    {
        if (receiver.crouchInput == false)
        {
            return;
        }
        //Reset Input
        receiver.crouchInput = false;

        if (currentMovementState != MovementState.Crouching)
        {
            currentMovementState = MovementState.Crouching;
            animator.SetBool(CrouchParam , true);
            SetCollider(new Vector3(0, config.crouchColliderCenterY, 0), config.crouchHeight);
            return;
        }

        if (CanGetUp())
        {
            currentMovementState = MovementState.Idle;
            SetCollider(new Vector3(0, config.defaultColliderCenterY, 0), config.defaultHeight);
            animator.SetBool(CrouchParam , true);
        }
    }

    private void ProneToggle()
    {
        if (receiver.proneInput == false)
        {
            return;
        }

        //Reset
        receiver.proneInput = false;

        if (Mathf.Abs(slope) > config.maxProneSlope)
        {
            animator.SetBool(ProneParam, false);
            Debug.Log("Prone not allowed : Slope too steep");
            return;
        }

        if (currentMovementState != MovementState.Prone)
        {
            currentMovementState = MovementState.Prone;
            animator.SetBool(ProneParam, true);
            SetCollider(new Vector3(0, config.proneColliderCenterY, 0), config.proneHeight);
            return;
        }

        currentMovementState = MovementState.Idle;
        ResetToIdleCollider();

    }

    #endregion

    #region Helper

    private bool CanGetUp()
    {
        RaycastHit hit;
        float checkDistance = 1.0f; // Adjust based on your character's height
        Vector3 origin = animator.GetBoneTransform(HumanBodyBones.Head).position + Vector3.up * _collider.height / 2f;
        bool isBlocked = Physics.Raycast(origin, Vector3.up, out hit, checkDistance);

        if (!isBlocked && (currentMovementState == MovementState.Crouching || currentMovementState == MovementState.Prone))
        {
            // Safe to get up
            // e.g., currentMovementState = MovementState.Walking;
            return true;
        }
        else
        {
            // Blocked, cannot get up
            return false;
        }
    }

    void PlayTargetAnimation(string name, bool isInteracting, bool isRootMotion = false)
    {
        animator.applyRootMotion = isRootMotion;
        animator.SetBool(InteractParam, isInteracting);
        animator.CrossFade(name, 0.2f);
    }
    
    void PlayTargetAnimation(string name , bool isInteracting , bool Overridable ,bool isRootMotion = false)
    {
        animator.SetBool("Overridable", Overridable);
        PlayTargetAnimation(name, isInteracting, isRootMotion);
    }

    private void SetCollider(Vector3 center, float height)
    {
        _collider.center = center;
        _collider.height = height;
    }

    private void ResetToIdleCollider()
    {
        SetCollider(new Vector3(0, config.defaultColliderCenterY, 0), config.defaultHeight);
    }

    public bool isGrounded(out RaycastHit hit)
    {
        Vector3 origin = transform.TransformPoint(0, config.offset, 0);
        bool sphereHit = Physics.SphereCast(origin, config.radius, Vector3.down, out hit, config.distance, config.groundLayer);
        return sphereHit;
    }

    public float ClampAbs(float value, float min)
    {
        if (Mathf.Abs(value) < min)
        {
            return 0f;
        }
        else
        {
            return value;
        }
    }

    private bool canSprint()
    {
        if(staminaHandler?.canuse == false)
        {
            return false;
        }

        switch (currentMovementState)
        {
            case MovementState.Crouching:
                return CanGetUp() == true ? true : false;
            case MovementState.Prone:
                return CanGetUp() == true ? true : false;

            default:
                return true;
        }
    }

   

    private void OnDrawGizmos() 
    {
        Gizmos.color = _grounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.TransformPoint(0 , config.offset , 0) , transform.position + Vector3.down * config.distance);
    }

    private bool ParamOn(string name)
    {
        return animator.GetBool(name);
    }

    #endregion

    [System.Serializable]
    public class InputReceiver
    {
        public Vector2 movementInput;
        public bool sprintInput;
        public bool jumpInput;
        public bool slideInput;
        public bool crouchInput;
        public bool proneInput;

    }
}
