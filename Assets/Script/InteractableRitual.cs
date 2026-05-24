using UnityEngine;
using System.Collections;

public class InteractableRitual : Interactable
{
    [Header("Ritual Settings")]
    public float waktuBakar = 2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip burnSound;

    private bool sudahHancur = false;
    private Renderer rend;

    public override (string, string) GetData()
    {
        if (sudahHancur)
            return ("Simbol Ritual", "Sudah hancur");

        if (RitualManager.Instance != null)
        {
            if (!RitualManager.Instance.hasBensin)
                return ("Simbol Ritual", "Butuh bensin");
            if (!RitualManager.Instance.hasKorek)
                return ("Simbol Ritual", "Butuh korek api");
        }

        return ("Simbol Ritual", "Bakar");
    }

    public override void Start()
    {
        base.Start();
        rend = GetComponent<Renderer>();
    }

    public override void OnInteract()
    {
        if (sudahHancur) return;

        if (RitualManager.Instance == null)
        {
            Debug.LogError("RitualManager tidak ditemukan di scene!");
            return;
        }

        if (!RitualManager.Instance.hasBensin)
        {
            Debug.Log("Belum punya bensin!");
            OnInteractFailed();
            return;
        }

        if (!RitualManager.Instance.hasKorek)
        {
            Debug.Log("Belum punya korek!");
            OnInteractFailed();
            return;
        }

        base.OnInteract();
        StartCoroutine(ProsesBakar());
    }

    IEnumerator ProsesBakar()
    {
        sudahHancur = true;
        Debug.Log("Ritual mulai terbakar...");

        if (audioSource != null && burnSound != null)
            audioSource.PlayOneShot(burnSound);

        if (rend != null) rend.material.color = new Color(1f, 0.4f, 0f);

        yield return new WaitForSeconds(waktuBakar / 2);

        if (rend != null) rend.material.color = Color.black;

        yield return new WaitForSeconds(waktuBakar / 2);

        OnInteractSucceed();
        RitualManager.Instance.RitualHancur();
        Destroy(gameObject, 0.1f);
    }
}