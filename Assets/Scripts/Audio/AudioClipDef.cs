using UnityEngine;

namespace StorageEscape.Audio
{
    /// <summary>
    /// Identificador de clips de audio definidos en el proyecto. Añade entradas aquí y crea un <see cref="AudioClipDef"/> por cada una.
    /// </summary>
    public enum AudioClipId
    {
        None = 0,
        Enviroment = 1,
        TapeExplotion = 2,
        Pickup = 3,
        BaseInteraction = 4,
        PickUpKey = 5,
        Switch = 6,
        FluorecentLight = 7,
        Scream = 8,
        DrawerOpen = 9,
        DoorCabinetOpen = 10,
        FixProjector = 11,
        PlayerWalking = 12,
        InstallingClockHand = 13,
        RotatingClockHand = 14,
    }

    /// <summary>
    /// Asocia un <see cref="AudioClip"/> con un <see cref="AudioClipId"/> para uso desde <see cref="AudioManager"/>.
    /// </summary>
    [CreateAssetMenu(
        fileName = "AudioClip",
        menuName = "Storage Escape/Audio/Clip",
        order = 0)]
    public class AudioClipDef : ScriptableObject
    {
        [SerializeField] private AudioClipId id = AudioClipId.None;
        [SerializeField] private AudioClip clip;
        [SerializeField, Range(0f, 1f)] private float volume = 1f;
        [SerializeField] private float pitchMin = 0.95f;
        [SerializeField] private float pitchMax = 1.05f;

        public AudioClipId Id => id;
        public AudioClip Clip => clip;
        public float Volume => volume;
        public float PitchMin => pitchMin;
        public float PitchMax => pitchMax;
    }
}
