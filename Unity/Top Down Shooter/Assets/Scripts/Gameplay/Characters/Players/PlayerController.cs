using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public PlayerInput playerInput;
    public float movementSpeed = 1f;

    private Rigidbody rb;
    [HideInInspector]
    public Vector3 rawInput;
    private Vector2 lookingPosition;
    private Camera _mainCamera;
    private Plane _groundPlane;
    
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        rb = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;
        _groundPlane = new Plane(Vector3.up, Vector3.zero);

    }

    // Update is called once per frame
    void Update()
    {
        OnDoMove(); //move the player
        OnLookToTarget(); //aiming the target
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

    public void OnLooking(InputAction.CallbackContext value)
    {
        lookingPosition = value.ReadValue<Vector2>();
    }

    protected virtual void OnLookToTarget()
    {
        var cameraRay = _mainCamera.ScreenPointToRay(lookingPosition);
        if (!_groundPlane.Raycast(cameraRay, out var rayLength)) return;
        var pointToLook = cameraRay.GetPoint(rayLength);
        var targetRotation = Quaternion.LookRotation(pointToLook - transform.position);
        targetRotation.x = 0;
        targetRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8f * Time.deltaTime);
    }

    public void OnShot(InputAction.CallbackContext value)
    {
        bool isPressing = value.ReadValue<float>() > 0;

        if(isPressing)
        {
            //Highlight direction
            OnAiming();
        }
        else
        {
            OnDoShot();
        }
    }

    protected virtual void OnAiming()
    {

    }

    protected virtual void OnDoShot()
    {

    }
}
