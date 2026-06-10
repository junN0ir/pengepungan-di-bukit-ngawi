using UnityEngine;
using System.Collections;

public class Act3Starter : MonoBehaviour
{
    [Header("Dialog Pembuka")]
    public DialogData[] dialogPembuka;

    [Header("Referensi")]
    public InteractableKertasTugas kertasTugas;
    public InteractablePintuKeluar pintuKeluar;

    void Start()
    {
        if (DialogSystem.Instance == null)
        {
            Debug.LogError("DialogSystem.Instance NULL!");
            return;
        }

        StartCoroutine(MulaiAct3());
    }

    IEnumerator MulaiAct3()
    {
        yield return new WaitForSeconds(1f);

        if (dialogPembuka.Length > 0)
            DialogSystem.Instance.TampilkanDialogBerantai(
                dialogPembuka,
                OnDialogPembukaSelesai
            );
        else
            OnDialogPembukaSelesai();
    }

    void OnDialogPembukaSelesai()
    {
        QuestManagerAct3.Instance.SetQuest(
            QuestManagerAct3.QuestState.MasukKelas
        );
        Debug.Log("Act3 dimulai");
    }
}