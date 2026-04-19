using UnityEngine;

namespace StorageEscape.Inventory
{
    /// <summary>
    /// Datos de un objeto que puede guardarse en el inventario y mostrarse en la barra de UI.
    /// Cada ranura admite como máximo una unidad de un ítem.
    /// </summary>
    [CreateAssetMenu(
        fileName = "InventoryItem",
        menuName = "Storage Escape/Inventario/Ítem",
        order = 0)]
    public class InventoryItemDefinition : ScriptableObject
    {
        [SerializeField] private string displayName = "Objeto";
        [SerializeField] private Sprite icon;

        public string DisplayName => displayName;
        public Sprite Icon => icon;
    }
}
