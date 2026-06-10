using UnityEngine;

public class InteractableMotor : Interactable
{
    [Header("Dialog Motor")]
    public DialogData[] dialogAmbilMotor;

    private bool bisaInteract = false;
    private bool sudahDiambil = false;

    public override void Start()
    {
        base.Start();
        objectName = "Kunci Motor";
        interactMessage = "Ambil";
    }

    public void AktifkanInteract()
    {
        bisaInteract = true;
        Debug.Log("Kunci motor bisa diambil");
    }

    public override (string, string) GetData()
    {
        if (sudahDiambil)
            return ("", "");

        if (!bisaInteract)
            return ("Kunci Motor", "...");

        return ("Kunci Motor", "Ambil");
    }

    public override void OnInteract()
    {
        if (DialogSystem.Instance != null && DialogSystem.Instance.IsDialogPlaying)
            return;

        if (!bisaInteract)
        {
            DialogSystem.Instance.TampilkanDialog(
                "Yono",
                "Belum perlu ambil kunci sekarang.",
                2f
            );
            return;
        }

        if (sudahDiambil) return;

        sudahDiambil = true;
        base.OnInteract();

        if (dialogAmbilMotor.Length > 0)
            DialogSystem.Instance.TampilkanDialogBerantai(dialogAmbilMotor, OnDialogMotorSelesai);
        else
            OnDialogMotorSelesai();
    }

    void OnDialogMotorSelesai()
    {
        QuestManager.Instance.SetQuest(QuestManager.QuestState.PergiKeKampus);
        gameObject.SetActive(false);
        OnInteractSucceed();
    }
}