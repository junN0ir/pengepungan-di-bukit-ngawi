using System.Collections;
using UnityEngine;

public class Jump : MonoBehaviour
{
    Rigidbody rb;
    public float jumpStrength = 2;
    public event System.Action Jumped;
    public KeyCode key = KeyCode.Space;

    [SerializeField, Tooltip("Prevents jumping when the transform is in mid-air.")]
    GroundCheck groundCheck;
    FirstPersonMovement fpp;
    bool isJumpPressed;
    bool isJumpButtonClicked;

    void Awake()
    {
        // Try to get groundCheck.
        groundCheck = GetComponentInChildren<GroundCheck>();
    }

    private void Start()
    {
        fpp = FirstPersonMovement.instance;
        rb = fpp.rb;
        fpp.jumpButton.onClick.AddListener(OnJumpButtonClicked);
    }

    void LateUpdate()
    {
        if (!fpp.useJoystick) isJumpPressed = Input.GetKeyDown(key);
        else isJumpPressed = isJumpButtonClicked;

        if (isJumpPressed && (!groundCheck || groundCheck.isGrounded))
        {
            rb.AddForce(Vector3.up * 100 * jumpStrength);
            Jumped?.Invoke();
        }
    }

    void OnJumpButtonClicked()
    {
        StartCoroutine(OnJumpButtonClickedDelay());
        IEnumerator OnJumpButtonClickedDelay()
        {
            isJumpButtonClicked = true;
            yield return new WaitForSeconds(0);
            isJumpButtonClicked = false;
        }
    }
}
