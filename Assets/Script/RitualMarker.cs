using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RitualMarker : MonoBehaviour
{
    [Header("Marker Settings")]
    public string labelMarker = "Ritual";
    public Color warnaMarker = new Color(1f, 0.3f, 0.1f);

    [Header("Debug")]
    public bool markerAktif = false;

    private GameObject markerObject;
    private RectTransform markerRect;
    private Image markerIcon;
    private TextMeshProUGUI markerText;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    public void AktifkanMarker(GameObject markerPrefab, Transform canvasParent)
    {
        if (markerAktif) return;

        markerObject = Instantiate(markerPrefab, canvasParent);
        markerRect = markerObject.GetComponent<RectTransform>();
        markerIcon = markerObject.GetComponentInChildren<Image>();
        markerText = markerObject.GetComponentInChildren<TextMeshProUGUI>();

        if (markerIcon != null) markerIcon.color = warnaMarker;
        if (markerText != null) markerText.text = labelMarker;

        markerAktif = true;
        markerObject.SetActive(true);
    }

    public void MatikanMarker()
    {
        if (!markerAktif) return;
        markerAktif = false;

        if (markerObject != null)
        {
            Destroy(markerObject);
            markerObject = null;
        }
    }

    void LateUpdate()
    {
        if (!markerAktif || markerObject == null || cam == null) return;

        UpdatePosisiMarker();
    }

    void UpdatePosisiMarker()
    {
        Vector3 screenPos = cam.WorldToScreenPoint(transform.position + Vector3.up * 2f);

        // Kalau di belakang kamera, sembunyikan
        if (screenPos.z < 0)
        {
            markerObject.SetActive(false);
            return;
        }

        markerObject.SetActive(true);

        // Hitung jarak untuk ukuran marker
        float jarak = Vector3.Distance(cam.transform.position, transform.position);
        float skala = Mathf.Clamp(300f / jarak, 0.5f, 1.5f);

        markerRect.localScale = Vector3.one * skala;

        // Clamp posisi supaya tidak keluar layar
        float padding = 50f;
        screenPos.x = Mathf.Clamp(screenPos.x, padding, Screen.width - padding);
        screenPos.y = Mathf.Clamp(screenPos.y, padding, Screen.height - padding);

        markerRect.position = screenPos;

        // Update teks jarak
        if (markerText != null)
            markerText.text = labelMarker + "\n" + Mathf.RoundToInt(jarak) + "m";
    }
}