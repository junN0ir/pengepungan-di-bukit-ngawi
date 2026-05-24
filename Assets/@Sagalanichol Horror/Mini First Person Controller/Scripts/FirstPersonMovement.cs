using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;

    [Header("Mobile")]
    public bool useJoystick;
    public SimpleTouchController moveJoystickController;
    public SimpleTouchController camTouchController;
    public Button jumpButton;
    public Button crouchButton;

    [HideInInspector] public Rigidbody rb;

    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();
    public static FirstPersonMovement instance;

    void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
        if (useJoystick) moveJoystickController.transform.parent.gameObject.SetActive(true);
        else moveJoystickController.transform.parent.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        // Get targetMovingSpeed.
        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        Vector2 targetVelocity;
        // Get targetVelocity from input.
        if (!useJoystick)
        {
            IsRunning = canRun && Input.GetKey(runningKey);
            targetVelocity = new Vector2(Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);
        }
        else
        {
            targetVelocity = new Vector2(moveJoystickController.GetTouchPosition.x * targetMovingSpeed, moveJoystickController.GetTouchPosition.y * targetMovingSpeed);
        }

        // Apply movement.
        rb.linearVelocity = transform.rotation * new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.y);
    }
}