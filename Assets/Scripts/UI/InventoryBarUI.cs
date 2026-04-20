using StorageEscape.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace StorageEscape.UI
{
    /// <summary>
    /// Barra horizontal de ranuras que refleja el <see cref="PlayerInventory"/> asignado.
    /// Asigna un array de <see cref="Image"/> (icono) en el mismo orden que las ranuras del inventario.
    /// Opcional: <see cref="slotSelectionMarkers"/> (un hijo por ranura) para resaltar la tecla 1–9 activa.
    /// </summary>
    public class InventoryBarUI : MonoBehaviour
    {
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private Image[] slotIcons;
        [Tooltip("Mismo orden que ranuras; se activa solo el de la ranura seleccionada.")]
        [SerializeField] private GameObject[] slotSelectionMarkers;
        [SerializeField] private Color emptySlotColor = new Color(1f, 1f, 1f, 0.2f);
        [SerializeField] private Color filledSlotColor = Color.white;

        private void OnEnable()
        {
            if (inventory != null)
            {
                inventory.InventoryChanged += Refresh;
                inventory.SelectedSlotChanged += Refresh;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (inventory != null)
            {
                inventory.InventoryChanged -= Refresh;
                inventory.SelectedSlotChanged -= Refresh;
            }
        }

        private void Refresh()
        {
            if (slotIcons == null || slotIcons.Length == 0)
            {
                return;
            }

            int max = inventory != null ? inventory.MaxSlots : 0;
            int selected = inventory != null ? inventory.SelectedSlotIndex : -1;

            UpdateSelectionMarkers(selected);

            for (int i = 0; i < slotIcons.Length; i++)
            {
                Image icon = slotIcons[i];
                if (icon == null)
                {
                    continue;
                }
                InventoryItemDefinition def = null;
                bool hasData = inventory != null
                    && i < max
                    && inventory.TryGetSlot(i, out def)
                    && def != null;

                if (hasData)
                {
                    icon.sprite = def.Icon;
                    icon.color = filledSlotColor;
                    icon.enabled = def.Icon != null;
                }
                else
                {
                    icon.sprite = null;
                    icon.color = emptySlotColor;
                    icon.enabled = true;
                }
            }
        }

        private void UpdateSelectionMarkers(int selectedSlotIndex)
        {
            if (slotSelectionMarkers == null || slotSelectionMarkers.Length == 0)
            {
                return;
            }

            for (int i = 0; i < slotSelectionMarkers.Length; i++)
            {
                GameObject marker = slotSelectionMarkers[i];
                if (marker == null)
                {
                    continue;
                }

                marker.SetActive(selectedSlotIndex >= 0 && i == selectedSlotIndex);
            }
        }
    }
}
