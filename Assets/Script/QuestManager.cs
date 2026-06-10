using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("UI Quest")]
    public TextMeshProUGUI questText;

    public enum QuestState
    {
        AwalGame,
        AmbilTugasDiMeja,
        AmbilHP,
        AmbilMotor,
        PergiKeKampus
    }

    public QuestState currentQuest = QuestState.AwalGame;

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
        Debug.Log("Quest berubah ke: " + quest);
    }

    void UpdateQuestUI()
    {
        if (questText == null) return;

        switch (currentQuest)
        {
            case QuestState.AwalGame:
                questText.text = "...";
                break;
            case QuestState.AmbilTugasDiMeja:
                questText.text = "Ambil tugas di meja";
                break;
            case QuestState.AmbilHP:
                questText.text = "Ambil HP di kasur";
                break;
            case QuestState.AmbilMotor:
                questText.text = "Ambil kunci motor";
                break;
            case QuestState.PergiKeKampus:
                questText.text = "Pergi ke kampus";
                break;
        }
    }
}