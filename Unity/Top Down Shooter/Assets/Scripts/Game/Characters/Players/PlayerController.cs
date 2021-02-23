using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInput playerInput;
    public float movementSpeed = 1f;

    //private variables
    Rigidbody rb;
    Vector3 rawInput;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        OnDoMove();
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
}
