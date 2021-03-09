using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerActions
{
    None,
    SimpleWeapon,
    Shotgun,
    Grenade,
    SpecialSkill
}

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public PlayerInput playerInput;
    public float movementSpeed = 1f;

    private PlayerActions _lastAction;

    private Rigidbody _rb;
    [HideInInspector]
    public Vector3 rawInput;
    private Vector2 _lookingPosition;
    private Camera _mainCamera;
    private Plane _groundPlane;
    
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        _rb = GetComponent<Rigidbody>();
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
        playerVelocity.y = _rb.velocity.y;
        _rb.velocity = (playerVelocity);
    }

    public void OnMovement(InputAction.CallbackContext value)
    {

        Vector2 inputMovement = value.ReadValue<Vector2>();

        rawInput = new Vector3(inputMovement.x, 0, inputMovement.y);
    }

    /// <summary>
    /// For Mobile Device Only, when click UI buttons select the pressed weapon, OnButtonUp perform Shot
    /// </summary>
    /// <param name="action">The button clicked</param>
    public void OnSelectAction(PlayerActions action)
    {
        if (action == PlayerActions.None)
            OnDoShot(_lastAction);

        _lastAction = action;
    }

    public void OnLooking(InputAction.CallbackContext value)
    {
        _lookingPosition = value.ReadValue<Vector2>();
    }

    protected virtual void OnLookToTarget()
    {
        if(playerInput.currentControlScheme == "Keyboard and Mouse")
        {
            var cameraRay = _mainCamera.ScreenPointToRay(_lookingPosition);
            if (!_groundPlane.Raycast(cameraRay, out var rayLength)) return;
            var pointToLook = cameraRay.GetPoint(rayLength);
            var targetRotation = Quaternion.LookRotation(pointToLook - transform.position);
            targetRotation.x = 0;
            targetRotation.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8f * Time.deltaTime);
        }
        else //Touchpad or Gamepad
        {
            if (_lookingPosition == Vector2.zero)
                return;

            Vector3 pointToLook = new Vector3(transform.position.x + _lookingPosition.x, transform.position.y, transform.position.z + _lookingPosition.y);
            var targetRotation = Quaternion.LookRotation(pointToLook - transform.position);
            targetRotation.x = 0;
            targetRotation.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8f * Time.deltaTime);

            OnAiming();
        }
    }

    /// <summary>
    /// For GamePad or Keyboard Only, next or previous weapon
    /// </summary>
    /// <param name="value">float value positive or negative</param>
    public void OnChangeWeapon(InputAction.CallbackContext value)
    {
        if(value.phase == InputActionPhase.Started)
        {
            int currentSelected = (int)_lastAction;
            currentSelected += (int)value.ReadValue<float>();

            if (currentSelected < 1)
                _lastAction = PlayerActions.SpecialSkill;
            else if (currentSelected > 4)
                _lastAction = PlayerActions.SimpleWeapon;
            else
                _lastAction = (PlayerActions)currentSelected;

            Debug.Log("Select " + _lastAction.ToString());
        }

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
            OnDoShot(_lastAction);
        }
    }

    protected virtual void OnAiming()
    {
        // TODO: Show aiming trail 
    }

    protected virtual void OnDoShot(PlayerActions actionToPerform)
    {
        if (actionToPerform == PlayerActions.None) //do nothing
            return;

        // TODO: Do Shot
        Debug.Log("Perform Shot with " + actionToPerform.ToString());
    }
}
