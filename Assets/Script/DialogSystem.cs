using UnityEngine;
using TMPro;
using System.Collections;

public class DialogSystem : MonoBehaviour
{
    public static DialogSystem Instance;

    [Header("UI")]
    public GameObject panelDialog;
    public TextMeshProUGUI textNama;
    public TextMeshProUGUI textDialog;
    public GameObject garisPemisah;

    public bool IsDialogPlaying { get; private set; }

    private Coroutine dialogCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (panelDialog != null) panelDialog.SetActive(false);
        IsDialogPlaying = false;
    }

    public void TampilkanDialog(string nama, string dialog, float durasi)
    {
        if (dialogCoroutine != null) StopCoroutine(dialogCoroutine);
        dialogCoroutine = StartCoroutine(DialogRoutine(nama, dialog, durasi));
    }

    public void TampilkanDialogBerantai(DialogData[] listDialog, System.Action onSelesai = null)
    {
        if (dialogCoroutine != null) StopCoroutine(dialogCoroutine);
        dialogCoroutine = StartCoroutine(DialogBerantaiRoutine(listDialog, onSelesai));
    }

    IEnumerator DialogRoutine(string nama, string dialog, float durasi)
    {
        IsDialogPlaying = true;

        if (panelDialog != null) panelDialog.SetActive(true);
        if (textNama != null) textNama.text = nama;
        if (textDialog != null) textDialog.text = dialog;

        yield return new WaitForSeconds(durasi);

        if (panelDialog != null) panelDialog.SetActive(false);
        IsDialogPlaying = false;
        dialogCoroutine = null;
    }

    IEnumerator DialogBerantaiRoutine(DialogData[] listDialog, System.Action onSelesai)
    {
        IsDialogPlaying = true;

        foreach (DialogData data in listDialog)
        {
            if (panelDialog != null) panelDialog.SetActive(true);
            if (textNama != null) textNama.text = data.nama;
            if (textDialog != null) textDialog.text = data.dialog;

            yield return new WaitForSeconds(data.durasi);
        }

        if (panelDialog != null) panelDialog.SetActive(false);
        IsDialogPlaying = false;
        dialogCoroutine = null;

        onSelesai?.Invoke();
    }
}