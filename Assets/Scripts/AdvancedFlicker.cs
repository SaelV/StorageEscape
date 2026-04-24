using UnityEngine;
using System.Collections;

public class AdvancedFlicker_NoUpdate : MonoBehaviour
{
    [Header("Lights")]
    public Light[] lights;

    [Header("Flicker Timing")]
    public float minFlickerTime = 0.05f;
    public float maxFlickerTime = 0.3f;

    [Header("Intensity Settings")]
    public float minIntensity = 0.2f;
    public float maxIntensity = 1.5f;
    public float intensityLerpDuration = 0.1f;

    [Header("Object Settings")]
    public GameObject targetObject;
    [Range(0f, 1f)]
    public float objectToggleChance = 0.2f;

    void Start()
    {
        // Start individual flicker routines for each light
        foreach (Light l in lights)
        {
            if (l != null)
                StartCoroutine(FlickerLight(l));
        }

        // Separate routine for object behavior
        if (targetObject != null)
            StartCoroutine(ObjectRoutine());
    }

    IEnumerator FlickerLight(Light l)
    {
        while (targetObject != null)
        {
            // Wait random time
            yield return new WaitForSeconds(Random.Range(minFlickerTime, maxFlickerTime));

            // Toggle ON/OFF occasionally
            if (Random.value > 0.3f)
                l.enabled = !l.enabled;

            // Smooth intensity change
            float startIntensity = l.intensity;
            float targetIntensity = Random.Range(minIntensity, maxIntensity);

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / intensityLerpDuration;
                l.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
                yield return null;
            }
        }

        l.intensity = Mathf.Lerp(l.intensity, 3, 0.3f);
    }

    IEnumerator ObjectRoutine()
    {
        while (targetObject != null)
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f));

            if (Random.value < objectToggleChance && targetObject)
            {
                targetObject.SetActive(!targetObject.activeSelf);
            }
        }
    }
}