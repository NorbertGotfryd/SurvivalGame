    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public LayerMask groundLayerMask;
    public float moveSpeed;
    public float jumpForce;

    private Vector2 currentMovementInput;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    public float lookSensitivity;
    [HideInInspector]
    public bool canLook;

    private Vector2 mouseDelta;
    private float cameraCurrentRotation;

    //components
    private Rigidbody rigbody;

    //singletion
    public static PlayerController instance;

    private void Awake()
    {
        instance = this;
        //get our components
        rigbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        canLook = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        if(canLook == true)
            CamerLook();
    }

    private void Move()
    {
        Vector3 direction = transform.forward * currentMovementInput.y + transform.right * currentMovementInput.x;
        direction *= moveSpeed;
        direction.y = rigbody.velocity.y;

        rigbody.velocity = direction;
    }

    private void CamerLook()
    {
        //look camera ion y axis
        cameraCurrentRotation += mouseDelta.y * lookSensitivity;
        cameraCurrentRotation = Mathf.Clamp(cameraCurrentRotation, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-cameraCurrentRotation, 0, 0);

        //look camera ion x axis
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            currentMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            currentMovementInput = Vector2.zero;
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (IsGrounded() == true)
            {
                rigbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    private bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(transform.position + (transform.forward * 0.2f) + (Vector3.up * 0.01f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.forward * 0.2f) + (Vector3.up * 0.01f), Vector3.down);
        Gizmos.DrawRay(transform.position + (transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down);
    }

    public void ToggleCursor(bool toggle)
    {
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}
