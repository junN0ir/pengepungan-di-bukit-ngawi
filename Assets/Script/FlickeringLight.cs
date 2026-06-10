using UnityEngine;
using System.Collections;

public class FlickeringLight : MonoBehaviour
{
    [Header("Flickering Settings")]
    public float minIntensity = 0f;
    public float maxIntensity = 1.5f;
    public float minFlickerTime = 0.05f;
    public float maxFlickerTime = 0.2f;
    public float normalTime = 2f;
    public float normalTimeVariance = 1f;

    [Header("Mode")]
    public bool flickerTerusMenerus = false;

    private Light cahaya;
    private float intensitasNormal;

    void Start()
    {
        cahaya = GetComponent<Light>();
        intensitasNormal = cahaya.intensity;
        StartCoroutine(FlickerRoutine());
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            if (flickerTerusMenerus)
            {
                yield return StartCoroutine(DoFlicker());
            }
            else
            {
                // Normal dulu beberapa detik
                cahaya.intensity = intensitasNormal;
                float waktuNormal = normalTime + Random.Range(
                    -normalTimeVariance,
                    normalTimeVariance
                );
                yield return new WaitForSeconds(waktuNormal);

                // Flicker sebentar
                yield return StartCoroutine(DoFlicker());
            }
        }
    }

    IEnumerator DoFlicker()
    {
        // Flicker 3-6 kali
        int jumlahFlicker = Random.Range(3, 7);

        for (int i = 0; i < jumlahFlicker; i++)
        {
            // Matikan
            cahaya.intensity = minIntensity;
            yield return new WaitForSeconds(
                Random.Range(minFlickerTime, maxFlickerTime)
            );

            // Nyalakan
            cahaya.intensity = Random.Range(
                intensitasNormal * 0.7f,
                maxIntensity
            );
            yield return new WaitForSeconds(
                Random.Range(minFlickerTime, maxFlickerTime)
            );
        }

        // Kembali normal
        cahaya.intensity = intensitasNormal;
    }
}