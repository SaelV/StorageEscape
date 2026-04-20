using UnityEngine;

namespace StorageEscape.Inventory
{
    /// <summary>
    /// Muestra el prefab de mano del <see cref="PlayerInventory.HeldItem"/> bajo un ancla (p. ej. hijo de la cámara).
    /// Asigna el mismo <see cref="PlayerInventory"/> que la barra y un transform de mano.
    /// </summary>
    public class PlayerHeldItemPresenter : MonoBehaviour
    {
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private Transform handAnchor;

        private GameObject currentInstance;

        private void OnEnable()
        {
            if (inventory != null)
            {
                inventory.InventoryChanged += RefreshHeldView;
                inventory.SelectedSlotChanged += RefreshHeldView;
            }

            RefreshHeldView();
        }

        private void OnDisable()
        {
            if (inventory != null)
            {
                inventory.InventoryChanged -= RefreshHeldView;
                inventory.SelectedSlotChanged -= RefreshHeldView;
            }

            ClearInstance();
        }

        private void RefreshHeldView()
        {
            ClearInstance();

            if (inventory == null || handAnchor == null)
            {
                return;
            }

            InventoryItemDefinition held = inventory.HeldItem;
            if (held == null)
            {
                return;
            }

            GameObject prefab = held.HeldViewPrefab;
            if (prefab == null)
            {
                return;
            }

            currentInstance = Instantiate(prefab, handAnchor);
            currentInstance.transform.localPosition = Vector3.zero;
            currentInstance.transform.localRotation = Quaternion.identity;
            currentInstance.transform.localScale = Vector3.one;
        }

        private void ClearInstance()
        {
            if (currentInstance != null)
            {
                Destroy(currentInstance);
                currentInstance = null;
            }
        }
    }
}
