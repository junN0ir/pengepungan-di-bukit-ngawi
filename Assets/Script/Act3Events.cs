using UnityEngine;

public class Act3Events : MonoBehaviour
{
    public InteractableKertasTugas kertasTugas;

    private bool sudahDibuka = false;

    public void OnPintuKelasDibuka()
    {
        // Kalau sudah pernah dipanggil, skip
        if (sudahDibuka) return;
        sudahDibuka = true;

        Debug.Log("Pintu kelas dibuka pertama kali");

        if (QuestManagerAct3.Instance != null)
            QuestManagerAct3.Instance.SetQuest(
                QuestManagerAct3.QuestState.AmbilTugas
            );

        if (DialogSystem.Instance != null)
        {
            DialogData[] dialog = new DialogData[]
            {
                new DialogData
                {
                    nama = "Yono",
                    dialog = "Ini kelasnya... tugasku harus ada di sini.",
                    durasi = 3f
                }
            };
            DialogSystem.Instance.TampilkanDialogBerantai(dialog, null);
        }
    }
}