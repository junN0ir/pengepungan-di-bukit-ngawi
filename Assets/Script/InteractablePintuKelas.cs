using UnityEngine;

public class InteractablePintuKelas : Interactable
{
    [Header("Referensi")]
    public InteractableKertasTugas kertasTugas;

    private bool sudahDibuka = false;

    public override void Start()
    {
        base.Start();
        objectName = "Pintu Kelas";
        interactMessage = "Masuk";
    }

    public override (string, string) GetData()
    {
        if (sudahDibuka)
            return ("Pintu Kelas", "Sudah terbuka");

        return ("Pintu Kelas", "Masuk");
    }

    public override void OnInteract()
    {
        if (DialogSystem.Instance != null && DialogSystem.Instance.IsDialogPlaying)
            return;

        if (sudahDibuka) return;

        sudahDibuka = true;
        base.OnInteract();
        OnInteractSucceed();

        // Update quest ke ambil tugas
        if (QuestManagerAct3.Instance != null)
            QuestManagerAct3.Instance.SetQuest(
                QuestManagerAct3.QuestState.AmbilTugas
            );

        // Aktifkan kertas bisa diinteract
        // Kertas dari awal sudah bisa diinteract
        // tapi kasih tahu player lewat quest text
        Debug.Log("Pintu kelas dibuka, ambil tugas");
    }
}