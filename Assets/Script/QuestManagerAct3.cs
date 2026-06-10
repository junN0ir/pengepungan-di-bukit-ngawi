using UnityEngine;
using TMPro;

public class QuestManagerAct3 : MonoBehaviour
{
    public static QuestManagerAct3 Instance;

    [Header("UI Quest")]
    public TextMeshProUGUI questText;

    public enum QuestState
    {
        MasukKelas,
        AmbilTugas,
        KeluarKampus
    }

    public QuestState currentQuest = QuestState.MasukKelas;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateQuestUI();
    }

    public void SetQuest(QuestState quest)
    {
        currentQuest = quest;
        UpdateQuestUI();
        Debug.Log("Quest Act3 berubah ke: " + quest);
    }

    void UpdateQuestUI()
    {
        if (questText == null) return;

        switch (currentQuest)
        {
            case QuestState.MasukKelas:
                questText.text = "Masuk ke kelas";
                break;
            case QuestState.AmbilTugas:
                questText.text = "Ambil tugas di meja kelas";
                break;
            case QuestState.KeluarKampus:
                questText.text = "Keluar dari kampus";
                break;
        }
    }
}