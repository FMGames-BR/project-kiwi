using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public enum PlayerActions
{
	None,
	SimpleWeapon,
	Shotgun,
	Grenade,
	SpecialSkill
}

public class PlayerController : CharacterBase
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

	[SerializeField] public LineRenderer attackLr;
	[SerializeField] public float attackTrailDistance = 1;
	[SerializeField] public Transform attackLookAtPoint;
	[SerializeField] public GameObject primaryAttack;
	private WeaponBase _primaryAttackWeapon;
	private RaycastHit _attackHit;
	private bool _mouseLeftButtonIsPressing = false;

	private string attackType = "throw";

	// throw attack --- start (i don't remember how to make region, but it's only temp)
	private int numberThrowPoints = 10;
	public Vector3[] throwPoints;
	[SerializeField] public LineRenderer throwAttackLr;
	private float throwPower = 2f;
	// throw attack --- end

	// Start is called before the first frame update
	void Awake()
	{
		instance = this;

		_rb = GetComponent<Rigidbody>();
		_mainCamera = Camera.main;
		_groundPlane = new Plane(Vector3.up, Vector3.zero);
		_mouseLeftButtonIsPressing = false;
		_primaryAttackWeapon = primaryAttack.GetComponent<WeaponBase>();
	}

	void Start()
	{
		throwAttackLr.positionCount = numberThrowPoints;
		throwPoints = new Vector3[numberThrowPoints - 1];

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

	/// <summary>
	/// For Mobile Device Only, when click UI buttons select the pressed weapon, OnButtonUp perform Shot
	/// </summary>
	/// <param name="action">The button clicked</param>
	public void OnSelectAction (PlayerActions action)
	{
		if (action == PlayerActions.None)
			OnDoShot(_lastAction);

		_lastAction = action;
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

	public void OnShot (InputAction.CallbackContext value)
	{
		bool isPressing = value.ReadValue<float>() > 0;
		if (isPressing) {
			//Highlight direction
			OnAiming();
			_mouseLeftButtonIsPressing = true;
		} else {
			OnDoShot(_lastAction);
			_mouseLeftButtonIsPressing = false;
		}
	}

	protected virtual void OnAiming()
	{
		var _t = transform;
		switch (attackType) {
		case "shoot":
			attackLr.gameObject.SetActive(true);
			//attackLookAtPoint.position
			attackLr.SetPosition(0, _t.position);
			if (Physics.Raycast(attackLookAtPoint.position, attackLookAtPoint.forward, out _attackHit, attackTrailDistance)) {
				attackLr.SetPosition(1, _attackHit.point);
			} else {
				attackLr.SetPosition(1, attackLookAtPoint.position + attackLookAtPoint.forward*attackTrailDistance);
			}
			break;

		case "throw":
			throwAttackLr.SetPosition(0, attackLookAtPoint.position);
			float throwHeight = 2.5f;
			throwPower = 2f;
			
			for (var i = 1; i < numberThrowPoints; i++) {
				var y = Mathf.Cos((throwPower)*(i*(1f/numberThrowPoints)))*(i*throwHeight);
				throwAttackLr.SetPosition(i, new Vector3(
					throwAttackLr.GetPosition(i - 1).x +_lookingPosition.x, 
					y,
					throwAttackLr.GetPosition(i - 1).z + (_lookingPosition.y)
				));
			}
			throwAttackLr.SetPosition(0, new Vector3(
				throwAttackLr.GetPosition(1).x -_lookingPosition.x, 
				0,
				throwAttackLr.GetPosition(1).z + (_lookingPosition.y)
				));
			break;

		default:
			break;
		}
		if (attackType == "throw") {

		}

		// TODO: Show aiming trail 
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="actionToPerform"></param>
	protected virtual void OnDoShot (PlayerActions actionToPerform)
	{
		_primaryAttackWeapon.OnAttack();

		if (actionToPerform == PlayerActions.None) // do nothing
			return;

		// TODO: Do Shot
		Debug.Log("Perform Shot with " + actionToPerform.ToString());

		if (playerInput.currentControlScheme == "Keyboard and Mouse") {
			attackLr.gameObject.SetActive(false);
		}

	}
}