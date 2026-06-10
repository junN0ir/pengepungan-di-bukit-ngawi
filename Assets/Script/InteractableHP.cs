using UnityEngine;

public class InteractableHP : Interactable
{
    [Header("Dialog HP")]
    public DialogData[] dialogAmbilHP;

    [Header("Referensi")]
    public InteractableMotor motorObject;

    private bool bisaInteract = false;
    private bool sudahDiambil = false;

    public override void Start()
    {
        base.Start();
        objectName = "HP";
        interactMessage = "Ambil";
    }

    public void AktifkanInteract()
    {
        bisaInteract = true;
        Debug.Log("HP bisa diambil");
    }

    public override (string, string) GetData()
    {
        if (sudahDiambil)
            return ("", "");

        if (!bisaInteract)
            return ("HP", "...");

        return ("HP", "Ambil");
    }

    public override void OnInteract()
    {
        if (DialogSystem.Instance != null && DialogSystem.Instance.IsDialogPlaying)
            return;

        if (!bisaInteract)
        {
            DialogSystem.Instance.TampilkanDialog(
                "Yono",
                "Belum perlu ambil HP sekarang.",
                2f
            );
            return;
        }

        if (sudahDiambil) return;

        sudahDiambil = true;
        base.OnInteract();

        if (dialogAmbilHP.Length > 0)
            DialogSystem.Instance.TampilkanDialogBerantai(dialogAmbilHP, OnDialogHPSelesai);
        else
            OnDialogHPSelesai();
    }

    void OnDialogHPSelesai()
    {
        if (motorObject != null)
            motorObject.AktifkanInteract();
        else
            Debug.LogWarning("motorObject belum di-assign!");

        QuestManager.Instance.SetQuest(QuestManager.QuestState.AmbilMotor);
        gameObject.SetActive(false);
        OnInteractSucceed();
    }
}