using UnityEngine;

public class InteractableBensin : Interactable
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pickupSound;

    private bool isCollected = false;

    public override (string, string) GetData()
    {
        return ("Kaleng Bensin", "Ambil");
    }

    public override void OnInteract()
    {
        if (isCollected) return;
        base.OnInteract();

        if (RitualManager.Instance == null)
        {
            Debug.LogError("RitualManager tidak ditemukan di scene!");
            return;
        }

        if (audioSource != null && pickupSound != null)
            audioSource.PlayOneShot(pickupSound);

        RitualManager.Instance.hasBensin = true;
        Debug.Log("Bensin diambil! hasBensin = true");

        isCollected = true;
        OnInteractSucceed();
        Destroy(gameObject, 0.5f);
    }
}