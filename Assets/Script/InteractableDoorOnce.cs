using UnityEngine;
using DG.Tweening;

public class InteractableDoorOnce : LockableInteractable
{
    [Header("Door")]
    public Transform doorPivot;
    public bool openAngleX = false;
    public bool openAngleY = true;
    public bool openAngleZ = false;
    public float openAngle = -90f;

    public bool openDirectionX = false;
    public bool openDirectionY = false;
    public bool openDirectionZ = true;
    public float openDirection = 1.5f;

    public float openCloseDuration = 0.3f;
    public bool useDirectionTween = false;

    [Header("Interact Sekali")]
    [Tooltip("Jika true, setelah pintu berhasil dibuka pertama kali, script ini akan dimatikan.")]
    public bool matikanScriptSetelahInteract = true;

    [Tooltip("Jika true, collider pintu ikut dimatikan agar tidak bisa ditarget interact lagi.")]
    public bool matikanColliderSetelahInteract = false;

    [Tooltip("Collider yang dipakai untuk interact. Jika kosong, script akan mencari Collider di object ini.")]
    public Collider colliderInteract;

    private Tweener currentTween;
    private Vector3 initialRotation;
    private Vector3 initialPosition;

    private bool sudahInteract = false;

    public override void Start()
    {
        base.Start();

        if (doorPivot != null)
        {
            initialRotation = doorPivot.localEulerAngles;
            initialPosition = doorPivot.localPosition;
        }

        if (colliderInteract == null)
        {
            colliderInteract = GetComponent<Collider>();
        }
    }

    public override void ToggleOpenClose()
    {
        if (sudahInteract)
        {
            return;
        }

        if (doorPivot == null)
        {
            Debug.LogWarning("Door Pivot belum diisi di InteractableDoorOnce.");
            return;
        }

        if (currentTween != null && currentTween.IsPlaying())
        {
            return;
        }

        sudahInteract = true;

        if (useDirectionTween)
        {
            OpenDoorPosition();
        }
        else
        {
            OpenDoorRotation();
        }

        OnOpened?.Invoke();

        if (matikanColliderSetelahInteract && colliderInteract != null)
        {
            colliderInteract.enabled = false;
        }

        if (matikanScriptSetelahInteract)
        {
            if (currentTween != null)
            {
                currentTween.OnComplete(() =>
                {
                    enabled = false;
                });
            }
            else
            {
                enabled = false;
            }
        }

        isOpen = true;
    }

    private void OpenDoorRotation()
    {
        Vector3 targetEulerAngles = initialRotation;

        if (openAngleX)
        {
            targetEulerAngles.x += openAngle;
        }
        else if (openAngleY)
        {
            targetEulerAngles.y += openAngle;
        }
        else if (openAngleZ)
        {
            targetEulerAngles.z += openAngle;
        }

        currentTween = doorPivot.DOLocalRotate(targetEulerAngles, openCloseDuration)
                                .SetEase(Ease.OutQuad);
    }

    private void OpenDoorPosition()
    {
        Vector3 targetPosition = initialPosition;

        if (openDirectionX)
        {
            targetPosition.x += openDirection;
        }
        else if (openDirectionY)
        {
            targetPosition.y += openDirection;
        }
        else if (openDirectionZ)
        {
            targetPosition.z += openDirection;
        }

        currentTween = doorPivot.DOLocalMove(targetPosition, openCloseDuration)
                                .SetEase(Ease.OutQuad);
    }

    public void ForceSetIsLocked()
    {
        isLocked = true;
        isRequiredItem = true;
        requiredItemName = "AWEDAADAFASFADGFASDFAEDFAEFDASDF";
    }
}