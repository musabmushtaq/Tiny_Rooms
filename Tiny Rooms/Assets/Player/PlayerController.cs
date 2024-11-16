using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement variables
    public float walkSpeed = 5f; // Walking speed
    public float sprintSpeed = 8f; // Sprinting speed
    public float crouchSpeed = 2.5f; // Crouching speed
    public float jumpForce = 5f; // Jump force
    public float gravity = -9.81f; // Gravity strength

    // Camera variables
    public Transform cameraTransform; // Reference to the player's camera transform
    public float mouseSensitivityX = 100f; // Mouse sensitivity for horizontal rotation
    public float mouseSensitivityY = 100f; // Mouse sensitivity for vertical rotation

    // Camera bobbing variables
    public float bobbingSpeed = 5f; // Speed of bobbing
    public float bobbingAmountHorizontal = 0.05f; // Horizontal bobbing amount
    public float bobbingAmountVertical = 0.02f; // Vertical bobbing amount
    public float sprintBobbingMultiplier = 1.5f; // Multiplier to increase bobbing when sprinting

    // Camera FOV adjustment for sprinting
    public Camera playerCamera; // Reference to the camera to change FOV
    public float defaultFOV = 60f; // Default field of view
    public float sprintFOV = 75f; // Field of view when sprinting
    public float fovTransitionSpeed = 5f; // Speed at which FOV transitions

    // Components
    private CharacterController characterController; // Reference to CharacterController component

    // State variables
    private Vector3 velocity; // To store the current velocity, including gravity
    private bool isGrounded; // To check if the player is grounded
    private float currentSpeed; // To store the current movement speed (walking, sprinting, crouching)
    private float bobbingTimer = 0f; // Timer to control the bobbing animation
    private float xRotation = 0f; // To store the camera's vertical rotation

    // Camera bobbing state
    private Vector3 cameraDefaultPosition; // To store the camera's initial position for resetting bobbing

    void Start()
    {
        // Initialize the character controller
        characterController = GetComponent<CharacterController>();

        // Store the camera's default position (for later use when resetting bobbing)
        if (cameraTransform != null)
        {
            cameraDefaultPosition = cameraTransform.localPosition;
        }

        // Lock the cursor and hide it to enhance immersion
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Handle movement, camera rotation, jumping, gravity, bobbing, and FOV adjustment
        HandleMovement();
        HandleMouseLook();
        HandleJumping();
        ApplyGravity();
        HandleCameraBobbing();
        HandleFOV();
    }

    // Handles player movement (walking, sprinting, crouching)
    void HandleMovement()
    {
        // Check if the player is grounded
        isGrounded = characterController.isGrounded;

        // If the player is grounded and falling, set a small downward force to keep them grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Get input for horizontal (left/right) and vertical (forward/backward) movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Create a movement vector based on input and transform it relative to the player's orientation
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Adjust movement speed based on sprinting or crouching
        if (Input.GetKey(KeyCode.LeftShift)) // Sprinting
        {
            currentSpeed = sprintSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl)) // Crouching
        {
            currentSpeed = crouchSpeed;
        }
        else // Walking
        {
            currentSpeed = walkSpeed;
        }

        // Move the player using the CharacterController
        characterController.Move(move * currentSpeed * Time.deltaTime);
    }

    // Handles camera rotation based on mouse input
    void HandleMouseLook()
    {
        // Get mouse input for horizontal and vertical camera rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;

        // Rotate the player horizontally (left/right)
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically (up/down) and clamp it to prevent flipping
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Prevent the camera from rotating too far
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Apply vertical rotation to camera
    }

    // Handles jumping mechanics
    void HandleJumping()
    {
        // Allow the player to jump only if they are on the ground
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Apply jump force using the physics formula for velocity
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    // Applies gravity to the player, pulling them down when not grounded
    void ApplyGravity()
    {
        // Apply gravity over time
        velocity.y += gravity * Time.deltaTime;

        // Move the player using the CharacterController, including gravity
        characterController.Move(velocity * Time.deltaTime);
    }

    // Handles the camera bobbing effect when the player is moving
    void HandleCameraBobbing()
    {
        // Make sure the camera reference is not null
        if (cameraTransform == null) return;

        // Determine if the player is moving (horizontal or vertical movement)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        bool isMoving = moveX != 0 || moveZ != 0;

        // Apply bobbing only when the player is moving and grounded
        if (isMoving && isGrounded)
        {
            // Increase the bobbing timer, considering sprinting for faster bobbing
            bobbingTimer += Time.deltaTime * (currentSpeed == sprintSpeed ? bobbingSpeed * sprintBobbingMultiplier : bobbingSpeed);

            // Calculate the horizontal and vertical bobbing offsets using sine and cosine for smooth movement
            float horizontalOffset = Mathf.Sin(bobbingTimer) * bobbingAmountHorizontal;
            float verticalOffset = Mathf.Cos(bobbingTimer * 2f) * bobbingAmountVertical; // Higher frequency for vertical movement

            // Apply the calculated bobbing offsets to the camera's position
            cameraTransform.localPosition = new Vector3(
                cameraDefaultPosition.x + horizontalOffset,
                cameraDefaultPosition.y + verticalOffset,
                cameraDefaultPosition.z
            );
        }
        else
        {
            // Smoothly return the camera to its default position when the player stops moving
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                cameraDefaultPosition,
                Time.deltaTime * bobbingSpeed
            );

            // Reset the bobbing timer when the player is stationary
            bobbingTimer = 0;
        }
    }

    // Adjust the camera's field of view based on whether the player is sprinting
    void HandleFOV()
    {
        // Make sure the playerCamera reference is not null
        if (playerCamera == null) return;

        // Set the target FOV based on whether the player is sprinting or not
        float targetFOV = (currentSpeed == sprintSpeed) ? sprintFOV : defaultFOV;

        // Smoothly transition the camera's FOV to the target FOV
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * fovTransitionSpeed);
    }
}
