using System.Collections;
using StorageEscape.Inventory;
using UnityEngine;

namespace StorageEscape.Interaction
{
    /// <summary>
    /// Primera interacción: activa el objetivo durante un tiempo breve y luego lo apaga.
    /// Segunda interacción: solo si el jugador tiene <see cref="InventoryItemId.projector_bulb"/>;
    /// consume el ítem, deja el objetivo activo y deshabilita más interacciones.
    /// </summary>
    public class Projector : MonoBehaviour, IInteractable
    {
        private enum Phase
        {
            AwaitingFirst,
            PulsingFirst,
            AwaitingSecond,
            Completed
        }

        [SerializeField] private string interactionPrompt = "Interactuar";
        [SerializeField] private GameObject target;
        [SerializeField] private float firstPulseDurationSeconds = 0.2f;
        [SerializeField] private GameObject blueKey;

        private Phase phase = Phase.AwaitingFirst;

        public string InteractionPrompt => interactionPrompt;
        private void Awake()
        {
            blueKey.SetActive(false);
            target.SetActive(false);
        }
        public bool CanInteract(GameObject interactor)
        {
            return phase switch
            {
                Phase.AwaitingFirst => true,
                Phase.AwaitingSecond => InteractorHasProjectorBulb(interactor),
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

                    phase = Phase.PulsingFirst;
                    StartCoroutine(FirstPulseRoutine());
                    break;

                case Phase.AwaitingSecond:
                    {
                        PlayerInventory inventory = FindInventory(interactor);
                        if (inventory == null || !inventory.HasItem(InventoryItemId.projector_bulb))
                        {
                            break;
                        }

                        if (!inventory.TryRemoveFirstWithItemId(InventoryItemId.projector_bulb))
                        {
                            break;
                        }

                        if (target != null)
                        {
                            target.SetActive(true);
                            blueKey.SetActive(true);
                        }

                        phase = Phase.Completed;
                        break;
                    }
            }
        }

        private static bool InteractorHasProjectorBulb(GameObject interactor)
        {
            if (interactor == null)
            {
                return false;
            }

            PlayerInventory inventory = FindInventory(interactor);
            return inventory != null && inventory.HasItem(InventoryItemId.projector_bulb);
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

                phase = Phase.AwaitingSecond;
            }
        }
    }
}
