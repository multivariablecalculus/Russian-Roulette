using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Animator animator;
    public float mouseSensitivity = 1f;
    public float jumpHeight = 2f;
    public Transform cameraTransform;
    public LayerMask chairLayer;

    private CharacterController controller;
    private float xRotation = 0f;
    private bool isSitting = false;
    private Transform currentChair;
    private Vector3 velocity;
    private bool isGrounded;
    private float gravity = -9.81f;

    private Vector2 moveInput;  // Stores movement input
    private Vector2 lookInput;  // Stores mouse look input
    private PlayerInput playerInput; // Reference to PlayerInput

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found on player!");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        animator.Play("Idle");
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        if (!isSitting)
        {
            MovePlayer();
            RotateCamera();
        }

        controller.Move(velocity * Time.deltaTime);

        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (velocity.y < 0)
        {
            velocity.y = -2f;
        }
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log("Current Animation: " + animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        }
    }

    // ✅ Handles movement using the New Input System
    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Debug.Log($"moveX: {moveX}, moveZ: {moveZ}"); // Debugging input values

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // ✅ Calculate movement speed properly
        float speed = new Vector2(moveX, moveZ).magnitude;

        // ✅ Ensure animation triggers properly
        if (animator != null)
        {
            animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime); // Adds smoothing
        }
    }

    // ✅ Handles camera rotation using the New Input System
    void RotateCamera()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    // ✅ Handles jump with the New Input System
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    // ✅ Handles movement input from the New Input System
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // ✅ Handles mouse look input from the New Input System
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    // ✅ Handles sitting logic with the New Input System
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed) TryToSit();
    }

    void TryToSit()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3f, chairLayer))
        {
            if (!isSitting)
            {
                currentChair = hit.transform;
                transform.position = currentChair.position + new Vector3(0, 0.5f, 0);
                transform.rotation = currentChair.rotation;
                isSitting = true;

                if (animator != null) animator.SetBool("IsSitting", true);
            }
            else
            {
                isSitting = false;
                if (animator != null) animator.SetBool("IsSitting", false);
            }
        }
    }
}
