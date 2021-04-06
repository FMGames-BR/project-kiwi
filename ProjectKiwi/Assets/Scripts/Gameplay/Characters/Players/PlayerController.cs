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

	public Weapon currentWeaponType;

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

	public WeaponBase selectedWeapon;
	public Transform weaponSpawnPoint;
	//private WeaponBase _primaryAttackWeapon;
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
        //_primaryAttackWeapon = selectedWeapon.GetComponent<WeaponBase>();

#if UNITY_STANDALONE || UNITY_EDITOR
		currentWeaponType = Weapon.Shotgun;
#endif
	}

    private void Start()
    {
		OnSpawnWeapon(currentWeaponType);
	}

    // Update is called once per frame
    void Update()
	{
		OnDoMove();       //move the player
		OnLookToTarget(); //aiming the target
	}

	private void OnSpawnWeapon(Weapon weaponType)
    {
		if (selectedWeapon != null)
			SpawnerController.instance.OnPoolingWeapons(selectedWeapon);

		selectedWeapon = SpawnerController.instance.OnSpawnWeapon(weaponType, weaponSpawnPoint);

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
    public void OnSelectAction (Weapon action)
	{
		if (action == Weapon.None)
			OnDoShot(currentWeaponType);

		currentWeaponType = action;
		OnSpawnWeapon(action);
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
		//Debug.Log(value.action.phase);
		if (value.action.phase != InputActionPhase.Started) //canceled means end of button pressed
			return;
		#region Keyboard
		if (playerInput.currentControlScheme == "Keyboard and Mouse")
		{
			//change selected attack variable = pressed number
			char pressedNumber = value.control.name[0];
			switch (pressedNumber)
			{
				case '1':
					currentWeaponType = Weapon.Shotgun;
					break;
				case '2':
					currentWeaponType = Weapon.Shotgun;
					break;
				case '3':
					currentWeaponType = Weapon.Grenade;
					break;
				case '4':
					currentWeaponType = Weapon.Bazooka;
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
			int currentSelected = (int)currentWeaponType;
			currentSelected += (int)value.ReadValue<float>();

			if (currentSelected < 1)
				currentWeaponType = Weapon.Bazooka;
			else if (currentSelected > 3)
				currentWeaponType = Weapon.Shotgun;
			else
				currentWeaponType = (Weapon)currentSelected;

			Debug.Log("Select " + currentWeaponType.ToString());
		}
		#endregion

		OnSpawnWeapon(currentWeaponType);
	}

	public void OnShot (InputAction.CallbackContext value)
	{
		bool isPressing = value.ReadValue<float>() > 0;
		if (isPressing) {
			//Highlight direction
			OnAiming();
			_mouseLeftButtonIsPressing = true;
		} else {
			OnDoShot(currentWeaponType);
			_mouseLeftButtonIsPressing = false;
		}
	}

	protected virtual void OnAiming()
	{
		selectedWeapon.OnCalculateAim(_lookingPosition);
		selectedWeapon.OnAim(attackLr, transform, attackLookAtPoint);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="actionToPerform"></param>
	protected virtual void OnDoShot (Weapon actionToPerform)
	{
		if (actionToPerform == Weapon.None) // do nothing
			return;

		selectedWeapon.OnAttack();
		//reset line renderer
		attackLr.enabled = false;
		Debug.Log("reset LR");

		// TODO: Do Shot
		Debug.Log("Perform Shot with " + actionToPerform.ToString());

		if (playerInput.currentControlScheme == "Keyboard and Mouse") {
			attackLr.gameObject.SetActive(false);
		}
	}
	
	
	/// <summary>
	/// Keyboard Only - select primary weapon to 2-8 builds (turrets/traps)
	/// </summary>
	/// <param name="value">float value positive or negative</param>
	public void OnSelectPrimaryButtonAction (InputAction.CallbackContext value)
	{
		if (value.phase == InputActionPhase.Started) {
			Debug.Log(value.ReadValue<bool>());
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