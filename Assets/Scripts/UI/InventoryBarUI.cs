using StorageEscape.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace StorageEscape.UI
{
    /// <summary>
    /// Barra horizontal de ranuras que refleja el <see cref="PlayerInventory"/> asignado.
    /// Asigna un array de <see cref="Image"/> (icono) en el mismo orden que las ranuras del inventario.
    /// </summary>
    public class InventoryBarUI : MonoBehaviour
    {
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private Image[] slotIcons;
        [SerializeField] private Color emptySlotColor = new Color(1f, 1f, 1f, 0.2f);
        [SerializeField] private Color filledSlotColor = Color.white;

        private void OnEnable()
        {
            if (inventory != null)
            {
                inventory.InventoryChanged += Refresh;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (inventory != null)
            {
                inventory.InventoryChanged -= Refresh;
            }
        }

        private void Refresh()
        {
            if (slotIcons == null || slotIcons.Length == 0)
            {
                return;
            }

            int max = inventory != null ? inventory.MaxSlots : 0;

            for (int i = 0; i < slotIcons.Length; i++)
            {
                Image icon = slotIcons[i];
                if (icon == null)
                {
                    continue;
                }

                bool hasData = inventory != null
                    && i < max
                    && inventory.TryGetSlot(i, out InventoryItemDefinition def)
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
    }
}
