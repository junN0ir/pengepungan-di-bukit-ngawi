using UnityEngine;
using TMPro;

public class KertasPetunjuk : Interactable
{
    [Header("Kertas Settings")]
    [TextArea(3, 10)]
    public string isiKertas = "Bakar simbol sekte dengan bensin dan api. Temukan 4 simbol di dalam hutan.";

    public GameObject panelKertas;
    public TextMeshProUGUI teksKertas;

    private bool sedangDibaca = false;

    public override void Start()
    {
        base.Start();
        objectName = "Kertas";
        interactMessage = "Baca";

        if (panelKertas != null) panelKertas.SetActive(false);
    }

    public override void OnInteract()
    {
        base.OnInteract();

        sedangDibaca = !sedangDibaca;

        if (panelKertas != null)
            panelKertas.SetActive(sedangDibaca);

        if (sedangDibaca && teksKertas != null)
            teksKertas.text = isiKertas;

        interactMessage = sedangDibaca ? "Tutup" : "Baca";

        Cursor.lockState = sedangDibaca ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = sedangDibaca;

        if (sedangDibaca) OnInteractSucceed();
    }
}