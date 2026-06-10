using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class CutscenePengintai : MonoBehaviour
{
    [Header("Kamera Utama Player")]
    public Camera kameraUtama;
    public Transform playerRoot;
    public Transform targetLihat;
    public float durasiPutarKamera = 1.5f;
    public float durasiDiamLihatTarget = 4f;

    [Header("Input / Cursor Lock")]
    public bool kunciCursorSaatCutscene = true;
    public bool sembunyikanCursorSaatCutscene = true;

    [Header("Spot Light Player")]
    public Light spotLightPlayer;

    [Header("Held Item — Item yang dipegang player")]
    public GameObject holdItemParent;

    [Header("Script Tambahan yang Dimatikan")]
    public Behaviour[] scriptDimatikanSaatCutscene;

    [Header("UI")]
    public GameObject canvasInteraction;
    public GameObject questPanel;
    public GameObject[] uiDimatikanSaatCutscene;
    public CanvasGroup[] canvasGroupDimatikanSaatCutscene;

    [Header("Kamera Cutscene")]
    public Camera kamera1;
    public Camera kamera2;
    public Camera kamera3;

    [Header("Durasi Per Kamera")]
    public float durasiKamera1 = 3f;
    public float durasiKamera2 = 3f;
    public float durasiKamera3 = 3f;

    [Header("Sosok Pengintai")]
    public GameObject sosokPengintai;

    [Header("Model MC Cutscene")]
    public GameObject modelMCCutscene;

    [Header("Fade Settings")]
    public CanvasGroup fadePanel;
    public float durasiGelapFade = 1f;

    [Header("Teks Narasi")]
    public TextMeshProUGUI teksCutscene;
    [TextArea(3, 6)]
    public string isiTeksCutscene = "Yono bergegas menuju motornya...\nIa tidak tahu bahwa malam ini\nakan mengubah segalanya.";
    public float durasiTampilTeks = 4f;

    [Header("Scene")]
    public string namaSceneBerikutnya = "SampleScene";

    private bool sedangBerjalan = false;
    private Quaternion rotasiAkhirPlayer;
    private Quaternion rotasiAkhirKameraLocal;
    private bool rotasiAkhirSudahAda = false;
    private bool rotasiSudahSelesai = false;

    void Start()
    {
        SetSemuaKameraMati();

        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
            fadePanel.gameObject.SetActive(false);
        }

        if (teksCutscene != null)
        {
            teksCutscene.alpha = 0f;
            teksCutscene.gameObject.SetActive(false);
        }

        if (sosokPengintai != null)
            sosokPengintai.SetActive(false);

        if (modelMCCutscene != null)
            modelMCCutscene.SetActive(false);
    }

    void LateUpdate()
    {
        if (!sedangBerjalan) return;

        PaksaMatikanUIGameplay();
        PaksaKunciCursor();

        // Setelah rotasi selesai, kunci posisi rotasi
        // Spotlight tidak ikut bergerak setelah lock
        if (rotasiAkhirSudahAda && !rotasiSudahSelesai) return;

        if (rotasiSudahSelesai && kameraUtama != null
            && kameraUtama.gameObject.activeSelf
            && playerRoot != null)
        {
            // Lock rotasi player dan kamera
            playerRoot.rotation = rotasiAkhirPlayer;
            kameraUtama.transform.localRotation = rotasiAkhirKameraLocal;

            // Spotlight TIDAK diupdate lagi setelah lock
            // sehingga tidak ikut bergerak
        }
    }

    void SetSemuaKameraMati()
    {
        if (kamera1 != null) kamera1.gameObject.SetActive(false);
        if (kamera2 != null) kamera2.gameObject.SetActive(false);
        if (kamera3 != null) kamera3.gameObject.SetActive(false);
    }

    public void MulaiCutscene()
    {
        if (sedangBerjalan) return;
        sedangBerjalan = true;
        StartCoroutine(CutsceneRoutine());
    }

    IEnumerator CutsceneRoutine()
    {
        PaksaKunciCursor();
        PaksaMatikanUIGameplay();

        // Matikan movement player
        if (FirstPersonMovement.instance != null)
            FirstPersonMovement.instance.enabled = false;

        // Matikan semua script gameplay
        MatikanScriptGameplay();

        // Sembunyikan item yang dipegang player
        if (holdItemParent != null)
        {
            holdItemParent.SetActive(false);
            Debug.Log("HoldItemParent disembunyikan");
        }
        else
        {
            // Coba cari lewat PlayerInteractionNoInventory
            if (PlayerInteractionNoInventory.Instance != null
                && PlayerInteractionNoInventory.Instance.holdItem != null)
            {
                PlayerInteractionNoInventory.Instance.holdItem.gameObject.SetActive(false);
                Debug.Log("HoldItem disembunyikan lewat PlayerInteraction");
            }
        }

        // Matikan spotlight player
        if (spotLightPlayer != null)
        {
            spotLightPlayer.enabled = false;
            Debug.Log("SpotLight player dimatikan");
        }

        // Aktifkan sosok pengintai
        if (sosokPengintai != null)
            sosokPengintai.SetActive(true);

        if (modelMCCutscene != null)
            modelMCCutscene.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        // Putar kamera ke target
        yield return StartCoroutine(PutarKameraPlayerKeTarget());

        // Setelah putar selesai, set flag lock
        rotasiSudahSelesai = true;

        // Diam menghadap sosok
        Debug.Log("Diam " + durasiDiamLihatTarget + " detik");
        yield return TungguSambilPaksaUIOff(durasiDiamLihatTarget);

        // Matikan kamera utama
        if (kameraUtama != null)
            kameraUtama.gameObject.SetActive(false);

        rotasiAkhirSudahAda = false;
        rotasiSudahSelesai = false;

        // Kamera 1
        SetSemuaKameraMati();
        if (kamera1 != null) kamera1.gameObject.SetActive(true);
        Debug.Log("Kamera 1");
        yield return TungguSambilPaksaUIOff(durasiKamera1);

        // Kamera 2
        SetSemuaKameraMati();
        if (kamera2 != null) kamera2.gameObject.SetActive(true);
        Debug.Log("Kamera 2");
        yield return TungguSambilPaksaUIOff(durasiKamera2);

        // Kamera 3
        SetSemuaKameraMati();
        if (kamera3 != null) kamera3.gameObject.SetActive(true);
        Debug.Log("Kamera 3");
        yield return TungguSambilPaksaUIOff(durasiKamera3);

        // Fade to black
        if (fadePanel != null)
            fadePanel.gameObject.SetActive(true);

        yield return StartCoroutine(FadeIn());

        SetSemuaKameraMati();

        // Teks narasi
        if (teksCutscene != null)
        {
            teksCutscene.gameObject.SetActive(true);
            teksCutscene.text = isiTeksCutscene;

            yield return StartCoroutine(FadeInTeks());
            yield return TungguSambilPaksaUIOff(durasiTampilTeks);
            yield return StartCoroutine(FadeOutTeks());

            teksCutscene.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(namaSceneBerikutnya);
    }

    IEnumerator TungguSambilPaksaUIOff(float durasi)
    {
        float timer = 0f;
        while (timer < durasi)
        {
            timer += Time.deltaTime;
            PaksaMatikanUIGameplay();
            PaksaKunciCursor();
            yield return null;
        }
    }

    void PaksaKunciCursor()
    {
        if (!kunciCursorSaatCutscene) return;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = !sembunyikanCursorSaatCutscene;
    }

    void MatikanScriptGameplay()
    {
        // Matikan FirstPersonLook
        FirstPersonLook fpsLook = FindObjectOfType<FirstPersonLook>();
        if (fpsLook != null) fpsLook.enabled = false;

        // Matikan script tambahan dari Inspector
        if (scriptDimatikanSaatCutscene != null)
        {
            foreach (Behaviour script in scriptDimatikanSaatCutscene)
            {
                if (script != null && script != this)
                    script.enabled = false;
            }
        }
    }

    void PaksaMatikanUIGameplay()
    {
        if (canvasInteraction != null) canvasInteraction.SetActive(false);
        if (questPanel != null) questPanel.SetActive(false);

        if (uiDimatikanSaatCutscene != null)
            foreach (GameObject ui in uiDimatikanSaatCutscene)
                if (ui != null) ui.SetActive(false);

        if (canvasGroupDimatikanSaatCutscene != null)
            foreach (CanvasGroup cg in canvasGroupDimatikanSaatCutscene)
                if (cg != null)
                {
                    cg.alpha = 0f;
                    cg.interactable = false;
                    cg.blocksRaycasts = false;
                }
    }

    IEnumerator PutarKameraPlayerKeTarget()
    {
        if (kameraUtama == null || playerRoot == null || targetLihat == null)
        {
            Debug.LogWarning("Ada field yang belum di-assign!");
            yield break;
        }

        Transform camTransform = kameraUtama.transform;
        Vector3 arahKeTarget = targetLihat.position - camTransform.position;
        Vector3 arahHorizontal = new Vector3(arahKeTarget.x, 0f, arahKeTarget.z);

        Quaternion rotasiAwalPlayer = playerRoot.rotation;
        Quaternion rotasiTargetPlayer = rotasiAwalPlayer;

        if (arahHorizontal.sqrMagnitude > 0.001f)
            rotasiTargetPlayer = Quaternion.LookRotation(
                arahHorizontal.normalized,
                Vector3.up
            );

        float jarakHorizontal = arahHorizontal.magnitude;
        float pitchTarget = -Mathf.Atan2(arahKeTarget.y, jarakHorizontal) * Mathf.Rad2Deg;
        pitchTarget = Mathf.Clamp(pitchTarget, -80f, 80f);

        float pitchAwal = camTransform.localEulerAngles.x;
        if (pitchAwal > 180f) pitchAwal -= 360f;

        float timer = 0f;
        while (timer < durasiPutarKamera)
        {
            timer += Time.deltaTime;
            PaksaMatikanUIGameplay();
            PaksaKunciCursor();

            float t = Mathf.Clamp01(timer / durasiPutarKamera);
            t = t * t * (3f - 2f * t);

            playerRoot.rotation = Quaternion.Slerp(
                rotasiAwalPlayer,
                rotasiTargetPlayer,
                t
            );

            float pitchSekarang = Mathf.LerpAngle(pitchAwal, pitchTarget, t);
            camTransform.localRotation = Quaternion.Euler(pitchSekarang, 0f, 0f);

            // Spotlight tidak diupdate saat rotasi berlangsung
            // karena sudah dimatikan di awal

            yield return null;
        }

        // Snap ke rotasi akhir
        rotasiAkhirPlayer = rotasiTargetPlayer;
        rotasiAkhirKameraLocal = Quaternion.Euler(pitchTarget, 0f, 0f);
        rotasiAkhirSudahAda = true;

        playerRoot.rotation = rotasiAkhirPlayer;
        camTransform.localRotation = rotasiAkhirKameraLocal;
    }

    IEnumerator FadeIn()
    {
        if (fadePanel == null) yield break;
        float timer = 0f;
        while (timer < durasiGelapFade)
        {
            timer += Time.deltaTime;
            fadePanel.alpha = Mathf.Clamp01(timer / durasiGelapFade);
            yield return null;
        }
        fadePanel.alpha = 1f;
    }

    IEnumerator FadeInTeks()
    {
        if (teksCutscene == null) yield break;
        float timer = 0f;
        float durasi = 1f;
        while (timer < durasi)
        {
            timer += Time.deltaTime;
            teksCutscene.alpha = Mathf.Clamp01(timer / durasi);
            yield return null;
        }
        teksCutscene.alpha = 1f;
    }

    IEnumerator FadeOutTeks()
    {
        if (teksCutscene == null) yield break;
        float timer = 0f;
        float durasi = 1f;
        while (timer < durasi)
        {
            timer += Time.deltaTime;
            teksCutscene.alpha = 1f - Mathf.Clamp01(timer / durasi);
            yield return null;
        }
        teksCutscene.alpha = 0f;
    }
}