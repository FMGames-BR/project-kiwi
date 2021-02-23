using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInput playerInput;
    public float movementSpeed = 1f;

    private Rigidbody rb;
    private Vector3 rawInput;
    private Vector2 aimingPosition;
    private Camera _mainCamera;
    private Plane _groundPlane;
    
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;
        _groundPlane = new Plane(Vector3.up, Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        OnDoMove(); //move the player
        OnDoAiming(); //aiming the target
    }

    protected virtual void OnDoMove()
    {
        Vector3 playerVelocity = (rawInput * movementSpeed);
        playerVelocity.y = rb.velocity.y;
        rb.velocity = (playerVelocity);
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>();

        rawInput = new Vector3(inputMovement.x, 0, inputMovement.y);
    }

    public void OnAiming(InputAction.CallbackContext value)
    {
        aimingPosition = value.ReadValue<Vector2>();
    }

    protected virtual void OnDoAiming()
    {
        var cameraRay = _mainCamera.ScreenPointToRay(aimingPosition);
        if (!_groundPlane.Raycast(cameraRay, out var rayLength)) return;
        var pointToLook = cameraRay.GetPoint(rayLength);
        var targetRotation = Quaternion.LookRotation(pointToLook - transform.position);
        targetRotation.x = 0;
        targetRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8f * Time.deltaTime);
    }
}
