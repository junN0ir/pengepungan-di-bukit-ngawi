using UnityEngine;
using System.Collections;

public class RitualObject : Interactable
{
    [Header("Ritual Settings")]
    public float waktuBakar = 2f;
    public bool sudahHancur = false;

    private Renderer rend;

    public override void Start()
    {
        base.Start();
        objectName = "Simbol Ritual";
        interactMessage = "Periksa";

        rend = GetComponent<Renderer>();
    }

    public override void OnInteract()
    {
        Debug.Log("RitualObject OnInteract dipanggil");

        base.OnInteract();

        if (sudahHancur)
        {
            Debug.Log("Ritual sudah hancur, skip");
            return;
        }

        if (RitualManager.Instance == null)
        {
            Debug.LogError("RitualManager.Instance null!");
            return;
        }

        Debug.Log("hasBensin: " + RitualManager.Instance.hasBensin + " | hasKorek: " + RitualManager.Instance.hasKorek);

        if (!RitualManager.Instance.hasBensin)
        {
            interactMessage = "Butuh bensin";
            OnInteractFailed();
            return;
        }

        if (!RitualManager.Instance.hasKorek)
        {
            interactMessage = "Butuh korek api";
            OnInteractFailed();
            return;
        }

        StartCoroutine(ProsesBakar());
    }

    IEnumerator ProsesBakar()
    {
        sudahHancur = true;
        interactMessage = "Terbakar...";

        if (rend != null) rend.material.color = new Color(1f, 0.4f, 0f);

        yield return new WaitForSeconds(waktuBakar / 2);

        if (rend != null) rend.material.color = Color.black;

        yield return new WaitForSeconds(waktuBakar / 2);

        Debug.Log("Ritual hancur, memanggil RitualHancur()");
        OnInteractSucceed();
        RitualManager.Instance.RitualHancur();
        gameObject.SetActive(false);
    }
}