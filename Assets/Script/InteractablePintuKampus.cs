using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractablePintuKampus : Interactable
{
    [Header("Scene Settings")]
    public string namaScene = "ActKampus";

    public override void Start()
    {
        base.Start();
        objectName = "Pintu";
        interactMessage = "Keluar";
    }

    public override (string, string) GetData()
    {
        if (!BisaKeluar())
            return ("Pintu", "Terkunci");

        return ("Pintu", "Keluar ke kampus");
    }

    public override void OnInteract()
    {
        // Blok kalau dialog sedang berjalan
        if (DialogSystem.Instance != null && DialogSystem.Instance.IsDialogPlaying)
            return;

        if (!BisaKeluar())
        {
            switch (QuestManager.Instance.currentQuest)
            {
                case QuestManager.QuestState.AwalGame:
                case QuestManager.QuestState.AmbilTugasDiMeja:
                    DialogSystem.Instance.TampilkanDialog(
                        "Yono",
                        "Aku harus periksa mejaku dulu.",
                        2.5f
                    );
                    break;
                case QuestManager.QuestState.AmbilHP:
                    DialogSystem.Instance.TampilkanDialog(
                        "Yono",
                        "Tunggu, aku harus ambil HP dulu.",
                        2.5f
                    );
                    break;
                case QuestManager.QuestState.AmbilMotor:
                    DialogSystem.Instance.TampilkanDialog(
                        "Yono",
                        "Aku harus ambil kunci motor dulu.",
                        2.5f
                    );
                    break;
            }
            return;
        }

        base.OnInteract();
        OnInteractSucceed();
        SceneManager.LoadScene(namaScene);
    }

    bool BisaKeluar()
    {
        if (DialogSystem.Instance != null && DialogSystem.Instance.IsDialogPlaying)
            return false;

        if (QuestManager.Instance == null) return false;

        return QuestManager.Instance.currentQuest == QuestManager.QuestState.PergiKeKampus;
    }
}