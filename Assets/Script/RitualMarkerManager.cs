using UnityEngine;

public class RitualMarkerManager : MonoBehaviour
{
    public static RitualMarkerManager Instance;

    [Header("Referensi")]
    public GameObject markerPrefab;
    public Transform canvasParent;

    [Header("Ritual Objects")]
    public RitualMarker[] semuaRitual;

    [Header("Debug")]
    public bool markerSudahDibuka = false;

    void Awake()
    {
        Instance = this;
    }

    public void BukaSemuaMarker()
    {
        if (markerSudahDibuka) return;
        markerSudahDibuka = true;

        foreach (RitualMarker ritual in semuaRitual)
        {
            if (ritual != null)
                ritual.AktifkanMarker(markerPrefab, canvasParent);
        }

        Debug.Log("Semua marker ritual dibuka");
    }

    public void MatikanMarkerIndex(int index)
    {
        if (index < 0 || index >= semuaRitual.Length) return;
        if (semuaRitual[index] != null)
            semuaRitual[index].MatikanMarker();
    }
}