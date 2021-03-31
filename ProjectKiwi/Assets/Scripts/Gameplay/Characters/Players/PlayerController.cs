using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

using BehaviorDesigner.Runtime.Tactical;
using Assets.Scripts.Enums;

public class PlayerController : CharacterBase, IDamageable
{
	public static PlayerController instance;

	public PlayerInput playerInput;
	public float movementSpeed = 1f;

	public PlayerWeapon playerWeapon;
	public PlayerAttackType attackType = PlayerAttackType.Shoot;

	private Rigidbody _rb;
	[HideInInspector]
	public Vector3 rawInput;
	private Vector2 _lookingPosition;
	private Camera _mainCamera;
	private Plane _groundPlane;

	public LineRenderer attackLr;
	[Header("Temp Shoot variables")]
	[SerializeField] public float attackTrailDistance = 1;
	[SerializeField] public Transform attackLookAtPoint;
	private RaycastHit _attackHit;

	[SerializeField] public GunBase selectedWeapon;
	private WeaponBase _primaryAttackWeapon;
	private bool _mouseLeftButtonIsPressing = false;

	//[SerializeField] public LineRenderer throwAttackLr;
	[Header("Temp Throw variables")]
	public Vector2 throwForce;
	public int throwPoints;
	public float throwSpacingPoint;

    protected override void Awake()
    {
        base.Awake();

        instance = this;

        _rb = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;
        _groundPlane = new Plane(Vector3.up, Vector3.zero);
        _mouseLeftButtonIsPressing = false;
        _primaryAttackWeapon = selectedWeapon.GetComponent<WeaponBase>();

#if UNITY_STANDALONE || UNITY_EDITOR
		playerWeapon = PlayerWeapon.PrimaryWeapon;
		attackType = PlayerAttackType.Shoot;
#endif
	}

	// Update is called once per frame
	void Update()
	{
		OnDoMove();       //move the player
		OnLookToTarget(); //aiming the target
	}

	protected virtual void OnDoMove()
	{
		Vector3 playerVelocity = (rawInput*movementSpeed);
		playerVelocity.y = _rb.velocity.y;
		_rb.velocity = (playerVelocity);
	}

	public void OnMovement (InputAction.CallbackContext value)
	{

		Vector2 inputMovement = value.ReadValue<Vector2>();

		rawInput = new Vector3(inputMovement.x, 0, inputMovement.y);
	}

	public void OnSelectAttack(InputAction.CallbackContext value)
	{
        #region Keyboard
        if (playerInput.currentControlScheme == "Keyboard and Mouse")
        {
			//change selected attack variable = pressed number
			char pressedNumber = value.control.name[0];
			switch(pressedNumber)
            {
				case '1':
					playerWeapon = PlayerWeapon.PrimaryWeapon;
					attackType = PlayerAttackType.Shoot;
					break;
				case '2':
					playerWeapon = PlayerWeapon.Shotgun;
					attackType = PlayerAttackType.Shoot;
					break;
				case '3':
					playerWeapon = PlayerWeapon.Grenade;
					attackType = PlayerAttackType.Throw;
					break;
				case '4':
					playerWeapon = PlayerWeapon.SpecialSkill;
					attackType = PlayerAttackType.Shoot;
					break;
				case '5':

					break;
				case '6':

					break;
				case '7':

					break;
				case '8':

					break;
				case '9':

					break;
				case '0':

					break;
			}
        }
        #endregion

        #region GamePad
        else if (playerInput.currentControlScheme == "GamePad")
        {
			//change selected attack variable +1 or -1
			//Used to select traps
        }
		#endregion

		Debug.Log("Select " + playerWeapon.ToString());
	}

    /// <summary>
    /// For Mobile Device Only, when click UI buttons select the pressed weapon, OnButtonUp perform Shot
    /// </summary>
    /// <param name="action">The button clicked</param>
    public void OnSelectAction (PlayerWeapon action)
	{
		if (action == PlayerWeapon.None)
			OnDoShot(playerWeapon);

		playerWeapon = action;
		attackType = (playerWeapon == PlayerWeapon.Grenade) ? PlayerAttackType.Throw : PlayerAttackType.Shoot;
	}

	public void OnLooking (InputAction.CallbackContext value)
	{
		_lookingPosition = value.ReadValue<Vector2>();
	}

	protected virtual void OnLookToTarget()
	{
		if (playerInput.currentControlScheme == "Keyboard and Mouse") {
			var cameraRay = _mainCamera.ScreenPointToRay(_lookingPosition);
			if (!_groundPlane.Raycast(cameraRay, out var rayLength)) return;
			var _pointToLook = cameraRay.GetPoint(rayLength);
			var targetRotation = Quaternion.LookRotation(_pointToLook - transform.position);
			targetRotation.x = 0;
			targetRotation.z = 0;
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8f*Time.deltaTime);
			_pointToLook.y = transform.position.y;
			attackLookAtPoint.rotation = Quaternion.LookRotation((_pointToLook - attackLookAtPoint.transform.position).normalized);
			attackLr.gameObject.SetActive(_mouseLeftButtonIsPressing);
			if (_mouseLeftButtonIsPressing) {
				OnAiming();
			}
		} else //Touchpad or Gamepad
		{
			if (_lookingPosition == Vector2.zero) {
				attackLr.gameObject.SetActive(false);
			} else {
				Vector3 _pointToLook = new Vector3(transform.position.x + _lookingPosition.x, transform.position.y, transform.position.z + _lookingPosition.y);
				var targetRotation = Quaternion.LookRotation(_pointToLook - transform.position);
				targetRotation.x = 0;
				targetRotation.z = 0;
				attackLookAtPoint.rotation = Quaternion.LookRotation((_pointToLook - attackLookAtPoint.transform.position).normalized);
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8f*Time.deltaTime);
				OnAiming();
			}
		}
	}

	/// <summary>
	/// For GamePad or Keyboard Only, next or previous weapon
	/// </summary>
	/// <param name="value">float value positive or negative</param>
	public void OnChangeWeapon (InputAction.CallbackContext value)
	{
		if (value.phase == InputActionPhase.Started) {
			int currentSelected = (int)playerWeapon;
			currentSelected += (int)value.ReadValue<float>();

			if (currentSelected < 1)
				playerWeapon = PlayerWeapon.SpecialSkill;
			else if (currentSelected > 4)
				playerWeapon = PlayerWeapon.PrimaryWeapon;
			else
				playerWeapon = (PlayerWeapon)currentSelected;

			attackType = (playerWeapon == PlayerWeapon.Grenade) ? PlayerAttackType.Throw : PlayerAttackType.Shoot;

			Debug.Log("Select " + playerWeapon.ToString());
		}

	}

	public void OnShot (InputAction.CallbackContext value)
	{
		bool isPressing = value.ReadValue<float>() > 0;
		if (isPressing) {
			//Highlight direction
			OnAiming();
			_mouseLeftButtonIsPressing = true;
		} else {
			OnDoShot(playerWeapon);
			_mouseLeftButtonIsPressing = false;
		}
	}

	protected virtual void OnAiming()
	{
		Transform _t = transform;
		switch (attackType) {
			case PlayerAttackType.Shoot:
				//if (!attackLr.enabled)
				//	attackLr.enabled = true;
				//attackLr.gameObject.SetActive(true);
				////attackLookAtPoint.position
				//attackLr.positionCount = 2;
				//attackLr.SetPosition(0, _t.position);
				//if (Physics.Raycast(attackLookAtPoint.position, attackLookAtPoint.forward, out _attackHit, attackTrailDistance)) {
				//	attackLr.SetPosition(1, _attackHit.point);
				//} else {
				//	attackLr.SetPosition(1, attackLookAtPoint.position + attackLookAtPoint.forward*attackTrailDistance);
				//}

				selectedWeapon.OnAim(attackLr, transform, attackLookAtPoint);
				break;
			case PlayerAttackType.Throw:
				if (throwPoints == 0)
					return;

				Vector2 aimingDelta = Vector2.zero;

				if (playerInput.currentControlScheme == "Keyboard and Mouse")
				{
					// Construct a ray from the current mouse coordinates
					Ray ray = Camera.main.ScreenPointToRay(_lookingPosition);
					RaycastHit hit;
					if (Physics.Raycast(ray, out hit))
					{
						Vector2 clickDelta = (new Vector2(hit.point.x, hit.point.z) - new Vector2(transform.position.x, transform.position.z));
						clickDelta.x = Mathf.Clamp(clickDelta.x, -throwForce.x, throwForce.x);
						clickDelta.y = Mathf.Clamp(clickDelta.y, -throwForce.x, throwForce.x);
						aimingDelta = clickDelta / throwForce.x;
					}
				}
				else //GamePad or Touch
				{
					aimingDelta = _lookingPosition;
				}

				if (!attackLr.enabled)
					attackLr.enabled = true;
				Vector3 _pointToLook = new Vector3(throwForce.x * aimingDelta.x, throwForce.y, throwForce.x * aimingDelta.y);
				Vector3[] points = Trajectory.OnUpdateTrajectory(transform.position, _pointToLook, throwPoints, throwSpacingPoint);
				attackLr.positionCount = points.Length + 1;
				attackLr.SetPosition(0, _t.position);

				for (int i = 0; i < points.Length; i++)
				{
					attackLr.SetPosition(i + 1, points[i]);
				}
				break;
			default:
				break;
		}

		// TODO: Show aiming trail 
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="actionToPerform"></param>
	protected virtual void OnDoShot (PlayerWeapon actionToPerform)
	{
		if (actionToPerform == PlayerWeapon.None) // do nothing
			return;

		_primaryAttackWeapon.OnAttack();
		//reset line renderer
		attackLr.enabled = false;
		Debug.Log("reset LR");

		// TODO: Do Shot
		Debug.Log("Perform Shot with " + actionToPerform.ToString());

		if (playerInput.currentControlScheme == "Keyboard and Mouse") {
			attackLr.gameObject.SetActive(false);
		}

	}

    public void Damage(float amount)
    {
        //throw new NotImplementedException();
    }

    public bool IsAlive()
    {
		//throw new NotImplementedException();
		return true;
    }
}