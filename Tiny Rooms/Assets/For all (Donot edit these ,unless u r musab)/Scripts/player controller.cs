using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//for the movement of the player (dont feel free to change this <3 ehehehe)

public class player_controller : MonoBehaviour
{
    // Components
    private CharacterController characterController;




    // Movement variables
    public float walkSpeed = 6f;
    public float sprintSpeed = 9f;
    public float crouchSpeed = 2.5f;
    public float jumpForce = 2f;
    public float gravity = -9.81f;
    private float gravityScaleAscending = 2f; // Gravity multiplier when going up
    private float gravityScaleDescending = 2f; // Gravity multiplier when falling down

   
    
    // Camera variables
    private Vector3 cameraDefaultPosition;
    public Transform cameraTransform;
    public float mouseSensitivityX = 100f; // Mouse sensitivity for horizontal rotation
    public float mouseSensitivityY = 100f; // Mouse sensitivity for vertical rotation
        //FOV adjustment for sprinting
        public Camera playerCamera;
        public float defaultFOV = 80f; // Normal field of view
        public float sprintFOV = 100f; // FOV while sprinting
        public float fovTransitionSpeed = 5f; // Speed of FOV transitions

    //Camera bobbing variables
    public float bobbingSpeed = 5f; // Speed of camera bobbing
    public float bobbingAmountHorizontal = 0.1f; // Horizontal bobbing amplitude
    public float bobbingAmountVertical = 0.05f; // Vertical bobbing amplitude
    public float sprintBobbingMultiplier = 1.5f; // Bobbing speed multiplier during sprinting
      
        // Jump landing bobbing variables
        public float landingBobbingAmount = 0.1f; // Vertical bobbing when landing
        public float landingBobbingSpeed = 6f; // Speed of landing bobbing recovery
      
        //state
        private bool landingEffectActive = false;
        private bool hasJumped = false;


    // State variables
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;
    private float bobbingTimer = 0f;
    private float xRotation = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (cameraTransform != null)
        {
            cameraDefaultPosition = cameraTransform.localPosition;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleJumping();
        ApplyGravity();
        HandleCameraBobbing();
        HandleFOV();
    }



    void HandleMovement()
    {
        // Grounded check
        isGrounded = characterController.isGrounded;
        if (isGrounded)
        {
            if (velocity.y < 0)
            {
                velocity.y = -2f;

                // Trigger landing effect only after a jump
                if (hasJumped && !landingEffectActive)
                {
                    StartCoroutine(LandingBobbingEffect());
                    hasJumped = false; // Reset jump state after landing
                }
            }
        }

        // Input for movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Determine movement speed
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            currentSpeed = crouchSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        // Move the player
        characterController.Move(move * currentSpeed * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;

        // Rotate the player horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Prevent flipping
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleJumping()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity); // Initial upward force
            hasJumped = true; // Mark as jumped
        }
    }

    void ApplyGravity()
    {
        // Simulate non-linear gravity effect
        if (velocity.y > 0) // Ascending
        {
            velocity.y += gravity * gravityScaleAscending * Time.deltaTime; // Slower gravity upward
        }
        else // Descending
        {
            velocity.y += gravity * gravityScaleDescending * Time.deltaTime; // Faster gravity downward
        }

        characterController.Move(velocity * Time.deltaTime);
    }

    void HandleCameraBobbing()
    {
        if (cameraTransform == null) return;

        // Determine if the player is moving
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        bool isMoving = moveX != 0 || moveZ != 0;

        if (isMoving && isGrounded)
        {
            // Increment bobbing timer
            bobbingTimer += Time.deltaTime * (currentSpeed == sprintSpeed ? bobbingSpeed * sprintBobbingMultiplier : bobbingSpeed);

            // Calculate bobbing offsets
            float horizontalOffset = Mathf.Sin(bobbingTimer) * bobbingAmountHorizontal;
            float verticalOffset = Mathf.Cos(bobbingTimer * 2f) * bobbingAmountVertical; // Higher frequency for vertical movement

            // Apply offsets to camera position
            cameraTransform.localPosition = new Vector3(
                cameraDefaultPosition.x + horizontalOffset,
                cameraDefaultPosition.y + verticalOffset,
                cameraDefaultPosition.z
            );
        }
        else if (!landingEffectActive)
        {
            // Smoothly return camera to default position
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                cameraDefaultPosition,
                Time.deltaTime * bobbingSpeed
            );
            bobbingTimer = 0; // Reset timer when stationary
        }
    }

    IEnumerator LandingBobbingEffect()
    {
        landingEffectActive = true;

        // Bob the camera down slightly
        float elapsed = 0f;
        while (elapsed < 0.1f)
        {
            cameraTransform.localPosition = Vector3.Lerp(cameraDefaultPosition,
                cameraDefaultPosition - new Vector3(0, landingBobbingAmount, 0),
                elapsed / 0.1f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Return the camera to its default position
        elapsed = 0f;
        while (elapsed < 0.1f)
        {
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                cameraDefaultPosition,
                elapsed / 0.1f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        landingEffectActive = false;
    }

    void HandleFOV()
    {
        if (playerCamera == null) return;

        float targetFOV = (currentSpeed == sprintSpeed) ? sprintFOV : defaultFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * fovTransitionSpeed);
    }

}
