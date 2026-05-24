using UnityEngine;
using TMPro;

public class RitualManager : MonoBehaviour
{
    public static RitualManager Instance;

    [Header("Ritual Counter")]
    public int totalRitual = 4;
    public int ritualDestroyed = 0;

    [Header("Status Item")]
    public bool hasBensin = false;
    public bool hasKorek = false;

    [Header("UI")]
    public TextMeshProUGUI counterText;
    public GameObject escapePanel;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
        if (escapePanel != null) escapePanel.SetActive(false);
    }

    private void Update()
    {
        if (PlayerInteractionNoInventory.Instance == null) return;

        Item heldItem = PlayerInteractionNoInventory.Instance.holdItem;

        if (heldItem != null)
        {
            Debug.Log("Holding: '" + heldItem.objectName + "'");
            hasKorek = heldItem.objectName == "Korek Api";
        }
        else
        {
            hasKorek = false;
        }
    }

    public void RitualHancur()
    {
        ritualDestroyed++;
        UpdateUI();
        Debug.Log("Ritual hancur: " + ritualDestroyed + "/" + totalRitual);

        if (ritualDestroyed >= totalRitual)
        {
            if (escapePanel != null) escapePanel.SetActive(true);
            Debug.Log("Semua ritual hancur!");
        }
    }

    private void UpdateUI()
    {
        if (counterText == null)
        {
            Debug.LogError("counterText belum di-assign!");
            return;
        }
        counterText.text = ritualDestroyed + " / " + totalRitual;
    }
}