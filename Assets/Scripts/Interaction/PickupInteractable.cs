using StorageEscape.Inventory;
using UnityEngine;

namespace StorageEscape.Interaction
{
    /// <summary>
    /// Objeto recogible: al interactuar intenta pasar el ítem al <see cref="PlayerInventory"/> del interactor.
    /// </summary>
    public class PickupInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private InventoryItemDefinition item;
        [SerializeField] private string interactionPrompt = "Recoger";
        [SerializeField] private bool destroyRootOnPickup = true;

        public string InteractionPrompt => interactionPrompt;

        public bool CanInteract(GameObject interactor)
        {
            if (interactor == null || item == null)
            {
                return false;
            }

            PlayerInventory inventory = FindInventory(interactor);
            return inventory != null && inventory.CanAdd(item);
        }

        public void Interact(GameObject interactor)
        {
            if (interactor == null || item == null)
            {
                return;
            }

            PlayerInventory inventory = FindInventory(interactor);
            if (inventory == null)
            {
                Debug.LogWarning(
                    $"[PickupInteractable] No se encontró PlayerInventory en '{interactor.name}'. Añade el componente al jugador.",
                    this);
                return;
            }

            if (!inventory.TryAdd(item))
            {
                return;
            }

            if (destroyRootOnPickup)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private static PlayerInventory FindInventory(GameObject interactor)
        {
            return interactor.GetComponent<PlayerInventory>()
                ?? interactor.GetComponentInParent<PlayerInventory>()
                ?? interactor.GetComponentInChildren<PlayerInventory>();
        }
    }
}
