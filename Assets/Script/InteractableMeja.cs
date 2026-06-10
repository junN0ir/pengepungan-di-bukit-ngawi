using UnityEngine;

public class InteractableMeja : Interactable
{
    [Header("Dialog Meja")]
    public DialogData[] dialogMeja;

    [Header("Referensi")]
    public InteractableHP hpObject;

    private bool bisaInteract = false;
    private bool sudahInteract = false;

    public override void Start()
    {
        base.Start();
        objectName = "Tugas";
        interactMessage = "Periksa";
    }

    public void AktifkanInteract()
    {
        bisaInteract = true;
        Debug.Log("Meja bisa diinteract");
    }

    public override (string, string) GetData()
    {
        if (sudahInteract)
            return ("", "");

        if (!bisaInteract)
            return ("Tugas", "...");

        return ("Tugas", "Periksa");
    }

    public override void OnInteract()
    {
        // Blok kalau dialog sedang berjalan
        if (DialogSystem.Instance != null && DialogSystem.Instance.IsDialogPlaying)
            return;

        if (!bisaInteract)
        {
            DialogSystem.Instance.TampilkanDialog(
                "Yono",
                "Sepertinya tidak ada yang perlu dilakukan sekarang.",
                2f
            );
            return;
        }

        if (sudahInteract) return;

        sudahInteract = true;
        base.OnInteract();

        DialogSystem.Instance.TampilkanDialogBerantai(dialogMeja, OnDialogMejaSelesai);
    }

    void OnDialogMejaSelesai()
    {
        if (hpObject != null)
            hpObject.AktifkanInteract();

        QuestManager.Instance.SetQuest(QuestManager.QuestState.AmbilHP);
        OnInteractSucceed();
    }
}