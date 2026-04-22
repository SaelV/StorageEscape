using System;
using System.Collections;
using StorageEscape.Audio;
using StorageEscape.Inventory;
using UnityEngine;
using UnityEngine.Serialization;

namespace StorageEscape.Interaction
{
    /// <summary>
    /// Primera interacción: activa el objetivo durante un tiempo breve y luego lo apaga.
    /// Fase de reparación: el jugador entrega bombilla y cinta en interacciones separadas.
    /// Tras reparar: la llave azul aparece en cuanto están las dos piezas; la primera interacción
    /// enciende el objetivo; la segunda lo deja encendido y pasa a completado (sin más interacciones).
    /// </summary>
    public class Projector : MonoBehaviour, IInteractable
    {
        private enum Phase
        {
            AwaitingFirst,
            PulsingFirst,
            AwaitingSecond,
            AwaitingPostRepairFirstInteract,
            AwaitingPostRepairSecondInteract,
            Completed
        }

        [FormerlySerializedAs("interactionPrompt")]
        [SerializeField] private string promptEncender = "Encender";
        [SerializeField] private string promptPlaceBulb = "Poner bombilla";
        [SerializeField] private string promptPlaceTape = "Poner cinta";

        [SerializeField] private GameObject target;
        [SerializeField] private float firstPulseDurationSeconds = 0.2f;
        [SerializeField] private GameObject blueKey;
        [SerializeField] private GameObject tapeRecorder;
        [SerializeField] private ParticleSystem particleEffect;
        [SerializeField] private Collider tapeRecorderCollider;

        [Tooltip("Opcional. Si está vacío, se usa el primer PlayerInventory de la escena (un jugador).")]
        [SerializeField] private PlayerInventory playerInventory;

        private Phase phase = Phase.AwaitingFirst;
        private PlayerInventory cachedAutoResolvedInventory;
        private bool bulbDelivered;
        private bool tapeDelivered;

        public bool BulbDelivered => bulbDelivered;
        public bool TapeDelivered => tapeDelivered;
        public bool IsAwaitingRepairParts => phase == Phase.AwaitingSecond;

        public string InteractionPrompt
        {
            get
            {
                switch (phase)
                {
                    case Phase.AwaitingFirst:
                    case Phase.AwaitingPostRepairFirstInteract:
                        return promptEncender;
                    case Phase.AwaitingPostRepairSecondInteract:
                        return string.Empty;
                    case Phase.AwaitingSecond:
                        return BuildRepairPromptFromInventory(ResolvePlayerInventory());
                    default:
                        return string.Empty;
                }
            }
        }

        private void Awake()
        {
            if (blueKey != null)
            {
                blueKey.SetActive(false);
            }

            if (target != null)
            {
                target.SetActive(false);
            }
            particleEffect.Stop();
        }

        private PlayerInventory ResolvePlayerInventory()
        {
            if (playerInventory != null)
            {
                return playerInventory;
            }

            if (cachedAutoResolvedInventory == null)
            {
                cachedAutoResolvedInventory = FindFirstObjectByType<PlayerInventory>();
            }

            return cachedAutoResolvedInventory;
        }

        private string BuildRepairPromptFromInventory(PlayerInventory inventory)
        {
            if (inventory?.HeldItem == null)
            {
                return string.Empty;
            }

            InventoryItemId held = inventory.HeldItem.Id;
            if (!bulbDelivered && held == InventoryItemId.projector_bulb)
            {
                return promptPlaceBulb;
            }

            if (!tapeDelivered && held == InventoryItemId.record_tape)
            {
                return promptPlaceTape;
            }

            return string.Empty;
        }

        public bool CanInteract(GameObject interactor)
        {
            return phase switch
            {
                Phase.AwaitingFirst => true,
                Phase.AwaitingSecond => CanDeliverNextHeldRepairPart(interactor),
                Phase.AwaitingPostRepairFirstInteract => true,
                Phase.AwaitingPostRepairSecondInteract => true,
                _ => false,
            };
        }

        public void Interact(GameObject interactor)
        {
            switch (phase)
            {
                case Phase.AwaitingFirst:
                    if (target != null)
                    {
                        target.SetActive(true);
                    }
                    particleEffect.Play();
                    phase = Phase.PulsingFirst;
                    AudioManager.Instance.PlayClip(AudioClipId.BaseInteraction, transform.position);
                    AudioManager.Instance.PlayClipLoopForDuration(AudioClipId.FluorecentLight, transform.position, firstPulseDurationSeconds);
                    StartCoroutine(FirstPulseRoutine());
                    break;

                case Phase.AwaitingSecond:
                    {
                        PlayerInventory inventory = FindInventory(interactor);
                        if (inventory == null || !TryConsumeNextRepairPart(inventory))
                        {
                            break;
                        }

                        AudioManager.Instance.PlayClip(AudioClipId.FixProjector, transform.position);

                        if (bulbDelivered && tapeDelivered)
                        {
                            RevealBlueKey();

                            if (target != null)
                            {
                                target.SetActive(false);
                            }

                            phase = Phase.AwaitingPostRepairFirstInteract;
                        }

                        break;
                    }

                case Phase.AwaitingPostRepairFirstInteract:
                    AudioManager.Instance.PlayClip(AudioClipId.BaseInteraction, transform.position);
                    AudioManager.Instance.PlayClip(AudioClipId.FluorecentLight, transform.position, loop: true);
                    AudioManager.Instance.PlayClip(AudioClipId.Scream, transform.position);

                    if (target != null)
                    {
                        target.SetActive(true);
                    }
                    RevealBlueKey();

                    phase = Phase.Completed;
                    break;

               
            }
        }

        private bool CanDeliverNextHeldRepairPart(GameObject interactor)
        {
            if (interactor == null)
            {
                return false;
            }

            PlayerInventory inventory = FindInventory(interactor);
            if (inventory == null || inventory.HeldItem == null)
            {
                return false;
            }

            InventoryItemId held = inventory.HeldItem.Id;
            if (held == InventoryItemId.projector_bulb)
            {
                return !bulbDelivered;
            }

            if (held == InventoryItemId.record_tape)
            {
                return !tapeDelivered;
            }

            return false;
        }

        private bool TryConsumeNextRepairPart(PlayerInventory inventory)
        {
            if (inventory.HeldItem == null)
            {
                return false;
            }

            InventoryItemId held = inventory.HeldItem.Id;

            if (!bulbDelivered && held == InventoryItemId.projector_bulb)
            {
                if (!inventory.TryConsumeHeldItem(InventoryItemId.projector_bulb))
                {
                    return false;
                }

                bulbDelivered = true;

                return true;
            }

            if (!tapeDelivered && held == InventoryItemId.record_tape)
            {
                if (!inventory.TryConsumeHeldItem(InventoryItemId.record_tape))
                {
                    return false;
                }

                if (tapeRecorder != null)
                {
                    tapeRecorder.SetActive(true);
                }
                tapeDelivered = true;
                return true;
            }

            return false;
        }

        private void RevealBlueKey()
        {
            if (blueKey == null)
            {
                Debug.LogWarning(
                    "[Projector] Referencia 'blueKey' sin asignar: no se mostrará la llave azul.",
                    this);
                return;
            }

            blueKey.SetActive(true);
        }

        private static PlayerInventory FindInventory(GameObject interactor)
        {
            return interactor.GetComponent<PlayerInventory>()
                ?? interactor.GetComponentInParent<PlayerInventory>()
                ?? interactor.GetComponentInChildren<PlayerInventory>();
        }

        private IEnumerator FirstPulseRoutine()
        {
            yield return new WaitForSeconds(firstPulseDurationSeconds);

            if (phase == Phase.PulsingFirst)
            {
                if (target != null)
                {
                    target.SetActive(false);
                }

                if (tapeRecorder != null)
                {
                    tapeRecorder.SetActive(false);
                }
                if (tapeRecorderCollider != null)
                {
                    tapeRecorderCollider.enabled = true;
                }
                particleEffect.Stop();
                AudioManager.Instance.PlayClip(AudioClipId.TapeExplotion, transform.position);
                bulbDelivered = false;
                tapeDelivered = false;
                phase = Phase.AwaitingSecond;
            }
        }
    }
}
