using UnityEngine;

public class InteractableKertasTugas : Interactable
{
    [Header("Dialog")]
    public DialogData[] dialogAmbilTugas;

    [Header("Referensi")]
    public InteractablePintuKeluar pintuKeluar;

    private bool sudahDiambil = false;

    public override void Start()
    {
        base.Start();
        objectName = "Tugas";
        interactMessage = "Ambil";
    }

    public override (string, string) GetData()
    {
        if (sudahDiambil) return ("", "");
        return ("Tugas", "Ambil");
    }

    public override void OnInteract()
    {
        Debug.Log("Kertas OnInteract dipanggil");

        if (DialogSystem.Instance == null)
        {
            Debug.LogError("DialogSystem NULL! Pastikan ada object DialogSystem di scene ini.");
            return;
        }

        if (DialogSystem.Instance.IsDialogPlaying)
        {
            Debug.Log("Dialog sedang berjalan");
            return;
        }

        if (sudahDiambil)
        {
            Debug.Log("Sudah diambil");
            return;
        }

        if (dialogAmbilTugas == null || dialogAmbilTugas.Length == 0)
        {
            Debug.LogWarning("dialogAmbilTugas kosong! Isi di Inspector.");
            OnDialogSelesai();
            return;
        }

        Debug.Log("Mulai dialog kertas, jumlah: " + dialogAmbilTugas.Length);

        sudahDiambil = true;
        base.OnInteract();

        DialogSystem.Instance.TampilkanDialogBerantai(
            dialogAmbilTugas,
            OnDialogSelesai
        );
    }

    void OnDialogSelesai()
    {
        Debug.Log("Dialog kertas selesai");

        if (QuestManagerAct3.Instance != null)
            QuestManagerAct3.Instance.SetQuest(
                QuestManagerAct3.QuestState.KeluarKampus
            );
        else
            Debug.LogError("QuestManagerAct3.Instance NULL!");

        if (pintuKeluar != null)
            pintuKeluar.AktifkanPintu();
        else
            Debug.LogWarning("pintuKeluar belum di-assign!");

        if (transform.parent != null)
            transform.parent.gameObject.SetActive(false);
        else
            gameObject.SetActive(false);

        OnInteractSucceed();
    }
}