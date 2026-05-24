using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Ritual")]
    public int totalRitual = 4;
    public int ritualDestroyed = 0;

    [Header("Item Status")]
    public bool hasBensin = false;
    public bool hasKorek = false;

    [Header("UI")]
    public TextMeshProUGUI ritualCounterText;
    public GameObject escapePanel;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateRitualUI();
        if (escapePanel != null) escapePanel.SetActive(false);
    }

    public void RitualHancur()
    {
        ritualDestroyed++;
        UpdateRitualUI();

        if (ritualDestroyed >= totalRitual)
        {
            TriggerEscape();
        }
    }

    void UpdateRitualUI()
    {
        if (ritualCounterText != null)
            ritualCounterText.text = "Ritual: " + ritualDestroyed + " / " + totalRitual;
    }

    void TriggerEscape()
    {
        if (escapePanel != null) escapePanel.SetActive(true);
        Debug.Log("Semua ritual hancur! Kabur sekarang!");
    }
}