using StorageEscape.Inventory;
using UnityEngine;

namespace StorageEscape.UI
{
    /// <summary>
    /// Barra horizontal de ranuras que refleja el <see cref="PlayerInventory"/> asignado.
    /// Asigna un array de <see cref="InventorySlotUI"/> en el mismo orden que las ranuras del inventario.
    /// Cada elemento representa un slot disponible del inventario.
    /// </summary>
    public class InventoryBarUI : MonoBehaviour
    {
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private InventorySlotUI[] slotElements;

        private void Awake()
        {
            InitializeSlots();
        }

        private void OnEnable()
        {
            InitializeSlots();

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

        private void InitializeSlots()
        {
            if (slotElements == null || slotElements.Length == 0)
            {
                return;
            }

            for (int i = 0; i < slotElements.Length; i++)
            {
                InventorySlotUI slotElement = slotElements[i];
                if (slotElement == null)
                {
                    continue;
                }

                int slotNumber = i + 1;
                slotElement.SetSlot(slotNumber, null, false);
                slotElement.Clear();
            }
        }

        private void Refresh()
        {
            if (slotElements == null || slotElements.Length == 0)
            {
                return;
            }

            int max = inventory != null ? inventory.MaxSlots : 0;
            int selected = inventory != null ? inventory.SelectedSlotIndex : -1;

            for (int i = 0; i < slotElements.Length; i++)
            {
                InventorySlotUI slotElement = slotElements[i];
                if (slotElement == null)
                {
                    continue;
                }

                InventoryItemDefinition def = null;
                bool hasData = inventory != null
                    && i < max
                    && inventory.TryGetSlot(i, out def)
                    && def != null;

                int slotNumber = i + 1;
                Sprite icon = hasData ? def.Icon : null;
                bool isSelected = selected >= 0 && i == selected;
                slotElement.SetSlot(slotNumber, icon, isSelected);
            }
        }
    }
}
