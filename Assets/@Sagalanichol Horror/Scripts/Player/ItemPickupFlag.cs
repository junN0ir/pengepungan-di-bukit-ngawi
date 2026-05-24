using UnityEngine;

public enum JenisItemFlag { Bensin, Korek }

public class ItemPickupFlag : Interactable
{
    [Header("Item Flag Settings")]
    public JenisItemFlag jenisItem;

    public override void Start()
    {
        base.Start();

        if (jenisItem == JenisItemFlag.Bensin)
        {
            objectName = "Kaleng Bensin";
            interactMessage = "Ambil";
        }
        else
        {
            objectName = "Korek Api";
            interactMessage = "Ambil";
        }
    }

    public override void OnInteract()
    {
        Debug.Log("OnInteract dipanggil pada: " + objectName);

        base.OnInteract();

        if (RitualManager.Instance == null)
        {
            Debug.LogError("RitualManager.Instance null! Pastikan ada GameObject dengan script RitualManager di scene.");
            return;
        }

        if (jenisItem == JenisItemFlag.Bensin)
        {
            if (RitualManager.Instance.hasBensin)
            {
                interactMessage = "Sudah dimiliki";
                OnInteractFailed();
                return;
            }
            RitualManager.Instance.hasBensin = true;
            Debug.Log("hasBensin sekarang: " + RitualManager.Instance.hasBensin);
        }
        else
        {
            if (RitualManager.Instance.hasKorek)
            {
                interactMessage = "Sudah dimiliki";
                OnInteractFailed();
                return;
            }
            RitualManager.Instance.hasKorek = true;
            Debug.Log("hasKorek sekarang: " + RitualManager.Instance.hasKorek);
        }

        OnInteractSucceed();
        gameObject.SetActive(false);
    }
}