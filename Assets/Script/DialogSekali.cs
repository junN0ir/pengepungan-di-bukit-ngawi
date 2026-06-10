using UnityEngine;

public class DialogSekali : MonoBehaviour
{
    [Header("Dialog")]
    public DialogData[] listDialog;

    [Header("Quest setelah dialog (opsional)")]
    public bool ubahQuestSetelahDialog = false;
    public QuestManagerAct3.QuestState questSetelahDialog;

    private bool sudahMuncul = false;

    public void TampilkanDialogSekali()
    {
        // Kalau sudah pernah muncul, skip
        if (sudahMuncul) return;

        if (DialogSystem.Instance == null)
        {
            Debug.LogError("DialogSystem NULL!");
            return;
        }

        if (listDialog == null || listDialog.Length == 0)
        {
            Debug.LogWarning("listDialog kosong!");
            OnDialogSelesai();
            return;
        }

        sudahMuncul = true;
        Debug.Log("Dialog sekali ditampilkan");

        DialogSystem.Instance.TampilkanDialogBerantai(listDialog, OnDialogSelesai);
    }

    void OnDialogSelesai()
    {
        if (!ubahQuestSetelahDialog) return;
        if (QuestManagerAct3.Instance == null) return;
        QuestManagerAct3.Instance.SetQuest(questSetelahDialog);
    }
}