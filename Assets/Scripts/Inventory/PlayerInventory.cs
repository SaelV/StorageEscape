using System;
using UnityEngine;

namespace StorageEscape.Inventory
{
    /// <summary>
    /// Inventario del jugador: una ranura = como máximo un ítem.
    /// Añádelo al mismo GameObject raíz del jugador que usa <see cref="Interaction.PlayerInteractionController"/>.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField, Min(1)] private int maxSlots = 8;

        private InventoryItemDefinition[] slots;

        /// <summary>Se dispara cuando cambia el contenido.</summary>
        public event Action InventoryChanged;

        public int MaxSlots => slots != null ? slots.Length : maxSlots;

        private void Awake()
        {
            EnsureInitialized();
        }

        private void OnValidate()
        {
            if (maxSlots < 1)
            {
                maxSlots = 1;
            }
        }

        /// <summary>Indica si hay al menos una ranura libre.</summary>
        public bool CanAdd(InventoryItemDefinition definition)
        {
            if (definition == null)
            {
                return false;
            }

            EnsureInitialized();
            return FindEmptySlotIndex() >= 0;
        }

        /// <summary>Intenta colocar un ítem en la primera ranura vacía. Devuelve false si el inventario está lleno.</summary>
        public bool TryAdd(InventoryItemDefinition definition)
        {
            if (!CanAdd(definition))
            {
                return false;
            }

            EnsureInitialized();

            int index = FindEmptySlotIndex();
            slots[index] = definition;
            InventoryChanged?.Invoke();
            return true;
        }

        public bool TryGetSlot(int index, out InventoryItemDefinition definition)
        {
            definition = null;

            EnsureInitialized();
            if (index < 0 || index >= slots.Length)
            {
                return false;
            }

            definition = slots[index];
            return definition != null;
        }

        private int FindEmptySlotIndex()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null)
                {
                    return i;
                }
            }

            return -1;
        }

        private void EnsureInitialized()
        {
            if (slots != null && slots.Length == maxSlots)
            {
                return;
            }

            slots = new InventoryItemDefinition[maxSlots];
        }
    }
}
