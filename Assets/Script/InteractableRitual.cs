using UnityEngine;

public class InteractableRitual : Interactable
{
    [Header("Ritual Settings")]
    public bool sudahTerbakar = false;
    public int markerIndex = 0;

    [Header("Efek")]
    public GameObject objekApi;

    private Renderer rend;

    public override (string, string) GetData()
    {
        if (sudahTerbakar)
            return ("Simbol Ritual", "Sudah terbakar");

        if (RitualManager.Instance != null)
        {
            if (!RitualManager.Instance.hasBensin)
                return ("Simbol Ritual", "Butuh bensin");
            if (!RitualManager.Instance.hasKorek)
                return ("Simbol Ritual", "Butuh korek api");
        }

        return ("Simbol Ritual", "Bakar");
    }

    public override void Start()
    {
        base.Start();
        rend = GetComponent<Renderer>();
        if (objekApi != null) objekApi.SetActive(false);
    }

    public override void OnInteract()
    {
        if (sudahTerbakar) return;

        if (RitualManager.Instance == null)
        {
            Debug.LogError("RitualManager tidak ditemukan!");
            return;
        }

        if (!RitualManager.Instance.hasBensin) { OnInteractFailed(); return; }
        if (!RitualManager.Instance.hasKorek) { OnInteractFailed(); return; }

        base.OnInteract();

        if (objekApi != null) objekApi.SetActive(true);

        sudahTerbakar = true;
        OnInteractSucceed();
        RitualManager.Instance.RitualHancur();

        // Matikan marker ritual ini
        RitualMarker marker = GetComponent<RitualMarker>();
        if (marker != null)
            marker.MatikanMarker();
    }
}