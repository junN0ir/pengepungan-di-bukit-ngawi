using UnityEngine;
using TMPro;

public class KertasPetunjuk : Interactable
{
    [Header("Kertas Settings")]
    [TextArea(3, 10)]
    public string isiKertas = "Bakar simbol sekte dengan bensin dan api. Temukan 4 simbol di dalam hutan.";

    [Header("UI")]
    public GameObject panelKertas;
    public TextMeshProUGUI teksKertas;

    [Header("Marker")]
    public bool bukaMarkerSaatDibaca = true;

    private bool sedangDibaca = false;
    private bool sudahDibaca = false;
    private bool mauBuka = false;
    private int frameCounter = 0;

    private FirstPersonLook fpsLook;

    public override void Start()
    {
        base.Start();
        objectName = "Kertas";
        interactMessage = "Baca";
        if (panelKertas != null) panelKertas.SetActive(false);

        // Cari FirstPersonLook di scene
        fpsLook = FindObjectOfType<FirstPersonLook>();
    }

    public override void OnInteract()
    {
        if (sedangDibaca)
        {
            TutupKertas();
            return;
        }

        mauBuka = true;
        frameCounter = 0;

        base.OnInteract();
        OnInteractSucceed();
    }

    void Update()
    {
        if (mauBuka)
        {
            frameCounter++;
            if (frameCounter >= 2)
            {
                mauBuka = false;
                EksekusiBukaKertas();
            }
            return;
        }

        if (!sedangDibaca) return;

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
        {
            TutupKertas();
        }
    }

    void EksekusiBukaKertas()
    {
        sedangDibaca = true;
        interactMessage = "Tutup";

        if (panelKertas != null)
            panelKertas.SetActive(true);
        else
        {
            Debug.LogError("panelKertas NULL!");
            return;
        }

        if (teksKertas != null)
            teksKertas.text = isiKertas;
        else
            Debug.LogError("teksKertas NULL!");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Matikan gerakan badan
        if (FirstPersonMovement.instance != null)
            FirstPersonMovement.instance.enabled = false;

        // Matikan gerakan kamera
        if (fpsLook != null)
            fpsLook.enabled = false;

        Time.timeScale = 0f;
    }

    void TutupKertas()
    {
        sedangDibaca = false;
        interactMessage = "Baca";

        if (panelKertas != null)
            panelKertas.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Aktifkan kembali gerakan badan
        if (FirstPersonMovement.instance != null)
            FirstPersonMovement.instance.enabled = true;

        // Aktifkan kembali gerakan kamera
        if (fpsLook != null)
            fpsLook.enabled = true;

        // Buka marker setelah tutup kertas
        if (!sudahDibaca && bukaMarkerSaatDibaca)
        {
            sudahDibaca = true;
            if (RitualMarkerManager.Instance != null)
                RitualMarkerManager.Instance.BukaSemuaMarker();
            else
                Debug.LogWarning("RitualMarkerManager.Instance null!");
        }
    }
}