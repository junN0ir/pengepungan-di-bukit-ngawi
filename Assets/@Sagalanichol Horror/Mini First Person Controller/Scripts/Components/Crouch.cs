using System.Collections;
using UnityEngine;

public class Crouch : MonoBehaviour
{
    public KeyCode key = KeyCode.LeftControl;

    [Header("Slow Movement")]
    [Tooltip("Movement to slow down when crouched.")]
    public FirstPersonMovement movement;
    [Tooltip("Movement speed when crouched.")]
    public float movementSpeed = 2;

    [Header("Low Head")]
    [Tooltip("Head to lower when crouched.")]
    public Transform headToLower;
    [HideInInspector]
    public float? defaultHeadYLocalPosition;
    public float crouchYHeadPosition = 1;
    public float checkAbove = 1;

    [Tooltip("Collider to lower when crouched.")]
    public CapsuleCollider colliderToLower;
    [HideInInspector]
    public float? defaultColliderHeight;

    public bool IsCrouched { get; private set; }
    public event System.Action CrouchStart, CrouchEnd;

    FirstPersonMovement fpp;
    bool toggleState = false;
    bool isCrouchPressed;
    bool isCrouchButtonPressed;

    void Awake()
    {
        movement = GetComponentInParent<FirstPersonMovement>();
        headToLower = movement.GetComponentInChildren<Camera>().transform;
        colliderToLower = movement.GetComponentInChildren<CapsuleCollider>();
    }

    private void Start()
    {
        fpp = FirstPersonMovement.instance;
        fpp.crouchButton.onClick.AddListener(OnCrouchButtonClicked);
    }

    void OnCrouchButtonClicked()
    {
        StartCoroutine(OnCrouchButtonClickedDelay());
        IEnumerator OnCrouchButtonClickedDelay()
        {
            isCrouchButtonPressed = true;
            yield return new WaitForSeconds(0);
            isCrouchButtonPressed = false;
        }
    }

    void LateUpdate()
    {
        if (!fpp.useJoystick) isCrouchPressed = Input.GetKeyDown(key);
        else isCrouchPressed = isCrouchButtonPressed;

        if (isCrouchPressed)
        {
            toggleState = !toggleState;
            if (toggleState)
            {
                StartCrouch();
            }
            else
            {
                TryStand();
            }
        }
    }

    void StartCrouch()
    {
        // Lower head
        if (headToLower)
        {
            if (!defaultHeadYLocalPosition.HasValue)
            {
                defaultHeadYLocalPosition = headToLower.localPosition.y;
            }
            headToLower.localPosition = new Vector3(headToLower.localPosition.x, crouchYHeadPosition, headToLower.localPosition.z);
        }

        // Lower collider
        if (colliderToLower)
        {
            if (!defaultColliderHeight.HasValue)
            {
                defaultColliderHeight = colliderToLower.height;
            }
            float loweringAmount = defaultHeadYLocalPosition.HasValue
                ? defaultHeadYLocalPosition.Value - crouchYHeadPosition
                : defaultColliderHeight.Value * .5f;

            colliderToLower.height = Mathf.Max(defaultColliderHeight.Value - loweringAmount, 0);
            colliderToLower.center = Vector3.up * colliderToLower.height * .5f;
        }

        if (!IsCrouched)
        {
            IsCrouched = true;
            SetSpeedOverrideActive(true);
            CrouchStart?.Invoke();
        }
    }

    void TryStand()
    {
        if (Physics.Raycast(headToLower.position, transform.up, checkAbove))
        {
            Debug.Log("Cannot stand up, obstacle above!");
            return;
        }

        // Reset head position
        if (headToLower)
        {
            headToLower.localPosition = new Vector3(headToLower.localPosition.x, defaultHeadYLocalPosition.Value, headToLower.localPosition.z);
        }

        // Reset collider
        if (colliderToLower)
        {
            colliderToLower.height = defaultColliderHeight.Value;
            colliderToLower.center = Vector3.up * colliderToLower.height * .5f;
        }

        if (IsCrouched)
        {
            IsCrouched = false;
            SetSpeedOverrideActive(false);
            CrouchEnd?.Invoke();
        }
    }

    #region Speed override.
    void SetSpeedOverrideActive(bool state)
    {
        if (!movement)
        {
            return;
        }

        if (state)
        {
            if (!movement.speedOverrides.Contains(SpeedOverride))
            {
                movement.speedOverrides.Add(SpeedOverride);
            }
        }
        else
        {
            if (movement.speedOverrides.Contains(SpeedOverride))
            {
                movement.speedOverrides.Remove(SpeedOverride);
            }
        }
    }

    float SpeedOverride() => movementSpeed;
    #endregion
}
