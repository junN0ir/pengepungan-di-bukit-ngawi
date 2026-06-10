using UnityEngine;

public class InteractablePintuKeluar : Interactable
{
    [Header("Cutscene")]
    public CutscenePengintai cutscene;

    private bool bisaInteract = false;

    public override void Start()
    {
        base.Start();
        objectName = "Pintu Keluar";
        interactMessage = "Terkunci";
        bisaInteract = false;
    }

    public void AktifkanPintu()
    {
        bisaInteract = true;
        interactMessage = "Keluar";
        Debug.Log("Pintu keluar aktif");
    }

    public override (string, string) GetData()
    {
        if (!bisaInteract)
            return ("Pintu Keluar", "Terkunci");

        return ("Pintu Keluar", "Keluar");
    }

    public override void OnInteract()
    {
        if (DialogSystem.Instance != null && DialogSystem.Instance.IsDialogPlaying)
            return;

        if (!bisaInteract)
        {
            DialogSystem.Instance.TampilkanDialog(
                "Yono",
                "Aku harus ambil tugasku dulu.",
                2.5f
            );
            return;
        }

        base.OnInteract();
        OnInteractSucceed();

        if (cutscene != null)
            cutscene.MulaiCutscene();
        else
            Debug.LogError("Cutscene belum di-assign di Inspector!");
    }
}