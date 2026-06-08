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
        Debug.Log("RitualManager Awake - Instance set");
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
            hasKorek = heldItem.objectName == "Korek Api";
        else
            hasKorek = false;
    }

    public void RitualHancur()
    {
        ritualDestroyed++;
        UpdateUI();
        Debug.Log("RitualHancur dipanggil! Total: " + ritualDestroyed + "/" + totalRitual);

        if (EntityController.Instance == null)
        {
            Debug.LogError("EntityController.Instance NULL! Pastikan ada object Entity dengan script EntityController di scene.");
            return;
        }

        Debug.Log("Memanggil fase entitas ke-" + ritualDestroyed);

        switch (ritualDestroyed)
        {
            case 1: EntityController.Instance.OnRitual1Burned(); break;
            case 2: EntityController.Instance.OnRitual2Burned(); break;
            case 3: EntityController.Instance.OnRitual3Burned(); break;
            case 4: EntityController.Instance.OnRitual4Burned(); break;
            case 5: EntityController.Instance.OnRitual5Burned(); break;
            default: Debug.Log("Ritual ke-" + ritualDestroyed + " tidak ada fase entitas"); break;
        }

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