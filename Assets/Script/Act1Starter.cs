using UnityEngine;
using System.Collections;

public class Act1Starter : MonoBehaviour
{
    [Header("Dialog Pembuka")]
    public DialogData[] dialogPembuka;

    [Header("Referensi")]
    public InteractableMeja mejaTugas;
    public InteractableHP hpObject;
    public InteractableMotor motorObject;

    void Start()
    {
        Debug.Log("Act1Starter Start dipanggil");

        if (DialogSystem.Instance == null)
        {
            Debug.LogError("DialogSystem.Instance NULL!");
            return;
        }

        if (dialogPembuka.Length == 0)
        {
            Debug.LogWarning("Dialog Pembuka kosong! Langsung aktifkan meja.");
            OnDialogPembukaSelesai();
            return;
        }

        StartCoroutine(MulaiAct1());
    }

    IEnumerator MulaiAct1()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("Memulai dialog pembuka");
        DialogSystem.Instance.TampilkanDialogBerantai(dialogPembuka, OnDialogPembukaSelesai);
    }

    void OnDialogPembukaSelesai()
    {
        Debug.Log("Dialog pembuka selesai!");

        if (mejaTugas == null)
        {
            Debug.LogError("mejaTugas NULL!");
            return;
        }

        mejaTugas.AktifkanInteract();

        // Pass referensi motor ke HP supaya nanti bisa diaktifkan
        if (hpObject != null)
            hpObject.motorObject = motorObject;

        QuestManager.Instance.SetQuest(QuestManager.QuestState.AmbilTugasDiMeja);
    }
}