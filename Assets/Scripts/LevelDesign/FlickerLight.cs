using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickerLight : MonoBehaviour
{
    [Header("Target Light")]
    [Tooltip("If left empty the Light component on this GameObject will be used.")]
    [SerializeField] private Light targetLight;

    [Header("Base Intensity")]
    [Tooltip("Base intensity used as the baseline for flicker and oscillation.")]
    [SerializeField] private float baseIntensity = -1f; // if negative, will read from light on Start

    [Header("Random Flicker")]
    [Tooltip("Enable short randomized flickers (good for candles, torches, damaged lamps)")]
    [SerializeField] private bool enableRandomFlicker = true;
    [Tooltip("Minimum multiplier of the base intensity used for random flicker (0..1 to dim, >1 to brighten)")]
    [SerializeField] private float minIntensityMultiplier = 0.6f;
    [Tooltip("Maximum multiplier of the base intensity used for random flicker")]
    [SerializeField] private float maxIntensityMultiplier = 1.1f;
    [Tooltip("Minimum time between random flicker target changes (seconds)")]
    [SerializeField] private float flickerIntervalMin = 0.02f;
    [Tooltip("Maximum time between random flicker target changes (seconds)")]
    [SerializeField] private float flickerIntervalMax = 0.2f;
    [Tooltip("How quickly the intensity moves toward the random target (higher = snappier)")]
    [SerializeField][Range(0.1f, 50f)] private float flickerSmoothness = 15f;

    [Header("Smooth Oscillation / Change Over Time")]
    [Tooltip("Enable smooth intensity changes over time (sine or Perlin noise)")]
    [SerializeField] private bool enableSmoothOscillation = false;
    [Tooltip("Use Perlin noise for more natural variation; otherwise a sine wave is used")]
    [SerializeField] private bool usePerlinNoise = true;
    [Tooltip("Amplitude of the oscillation as a fraction of base intensity (e.g. 0.2 = ±20%)")]
    [SerializeField] private float oscillationAmplitude = 0.15f;
    [Tooltip("Frequency of the oscillation in Hz (cycles per second)")]
    [SerializeField] private float oscillationFrequency = 0.5f;
    [Tooltip("Seed offset for Perlin noise to get different patterns between lights")]
    [SerializeField] private float perlinSeed = 0f;

    // Internal state
    private float currentIntensityTarget;
    private float currentIntensity;
    private float nextFlickerTime;
    private float actualBaseIntensity;

    private void Awake()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        if (targetLight == null)
        {
            Debug.LogWarning("FlickerLight: No Light component found on target or GameObject.");
            enabled = false;
            return;
        }

        // initialize base intensity from inspector or from the light
        actualBaseIntensity = baseIntensity > 0f ? baseIntensity : targetLight.intensity;
        currentIntensity = actualBaseIntensity;
        currentIntensityTarget = actualBaseIntensity;
        ScheduleNextFlicker();
    }

    private void ScheduleNextFlicker()
    {
        nextFlickerTime = Time.time + Random.Range(flickerIntervalMin, flickerIntervalMax);
    }

    private void Update()
    {
        if (targetLight == null) return;

        // Random flicker logic: occasionally pick a new target multiplier
        if (enableRandomFlicker && Time.time >= nextFlickerTime)
        {
            float mult = Random.Range(minIntensityMultiplier, maxIntensityMultiplier);
            currentIntensityTarget = actualBaseIntensity * mult;
            ScheduleNextFlicker();
        }

        // Smoothly move current intensity toward the target (this smooths random jumps)
        if (enableRandomFlicker)
        {
            currentIntensity = Mathf.Lerp(currentIntensity, currentIntensityTarget, Time.deltaTime * flickerSmoothness);
        }
        else
        {
            // if random flicker disabled keep baseline
            currentIntensity = actualBaseIntensity;
        }

        // Oscillation / change over time
        float oscillationOffset = 0f;
        if (enableSmoothOscillation)
        {
            if (usePerlinNoise)
            {
                float n = Mathf.PerlinNoise(perlinSeed, Time.time * oscillationFrequency);
                // Normalize Perlin (0..1) to (-1..1)
                n = (n - 0.5f) * 2f;
                oscillationOffset = n * oscillationAmplitude * actualBaseIntensity;
            }
            else
            {
                float s = Mathf.Sin(Time.time * oscillationFrequency * Mathf.PI * 2f);
                oscillationOffset = s * oscillationAmplitude * actualBaseIntensity;
            }
        }

        // Final intensity (clamped to avoid negative values)
        float finalIntensity = Mathf.Max(0f, currentIntensity + oscillationOffset);
        targetLight.intensity = finalIntensity;
    }
}
