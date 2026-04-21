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
        [SerializeField] private InventoryItemId id = InventoryItemId.undefined;
        [SerializeField] private string displayName = "Objeto";
        [SerializeField] private Sprite icon;
        [Tooltip("Opcional: modelo en primera persona cuando esta ranura está seleccionada (1–9).")]
        [SerializeField] private GameObject heldViewPrefab;

        public InventoryItemId Id => id;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public GameObject HeldViewPrefab => heldViewPrefab;
    }
    public enum InventoryItemId {
        undefined,
        key_blue,
        key_red,
        key_green,
        key_cyan,
        key_purple,
        projector_bulb,
        record_tape
    }
}
