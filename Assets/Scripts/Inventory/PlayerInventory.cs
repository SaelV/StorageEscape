using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StorageEscape.Inventory
{
    /// <summary>
    /// Inventario del jugador: una ranura = como máximo un ítem.
    /// Teclas 1–9 eligen la ranura; el ítem activo está <see cref="HeldItem"/> (puede ser null si la ranura está vacía).
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        private static readonly Key[] HotbarKeys =
        {
            Key.Digit1,
            Key.Digit2,
            Key.Digit3,
            Key.Digit4,
            Key.Digit5,
            Key.Digit6,
            Key.Digit7,
            Key.Digit8,
            Key.Digit9,
        };

        [SerializeField, Min(1)] private int maxSlots = 9;
        [SerializeField] private bool useDigitKeysForHotbar = true;

        private InventoryItemDefinition[] slots;
        private int selectedSlotIndex = -1;

        /// <summary>Se dispara cuando cambia el contenido de las ranuras.</summary>
        public event Action InventoryChanged;

        /// <summary>Se dispara cuando cambia la ranura seleccionada (1–9) o su contenido afecta la selección.</summary>
        public event Action SelectedSlotChanged;

        public int MaxSlots => slots != null ? slots.Length : maxSlots;

        /// <summary>Ranura activa por teclado (0 = tecla 1, … 8 = tecla 9). -1 si aún no se eligió ninguna.</summary>
        public int SelectedSlotIndex => selectedSlotIndex;

        /// <summary>Ítem en la ranura seleccionada; null si no hay selección o la ranura está vacía.</summary>
        public InventoryItemDefinition HeldItem
        {
            get
            {
                if (selectedSlotIndex < 0 || selectedSlotIndex >= MaxSlots)
                {
                    return null;
                }

                return TryGetSlot(selectedSlotIndex, out InventoryItemDefinition def) ? def : null;
            }
        }

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

        private void Update()
        {
            if (!useDigitKeysForHotbar)
            {
                return;
            }

            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            EnsureInitialized();

            for (int i = 0; i < HotbarKeys.Length; i++)
            {
                if (i >= slots.Length)
                {
                    break;
                }

                if (keyboard[HotbarKeys[i]].wasPressedThisFrame)
                {
                    SelectHotbarSlot(i);
                }
            }
        }

        /// <summary>Selecciona la ranura <paramref name="hotbarIndex"/> (0 = tecla 1, … 8 = tecla 9).</summary>
        public void SelectHotbarSlot(int hotbarIndex)
        {
            EnsureInitialized();
            if (hotbarIndex < 0 || hotbarIndex >= slots.Length)
            {
                return;
            }

            if (selectedSlotIndex == hotbarIndex)
            {
                return;
            }

            selectedSlotIndex = hotbarIndex;
            SelectedSlotChanged?.Invoke();
        }

        /// <summary>Quita la selección de ranura (nada en mano hasta pulsar 1–9 de nuevo).</summary>
        public void ClearHotbarSelection()
        {
            if (selectedSlotIndex < 0)
            {
                return;
            }

            selectedSlotIndex = -1;
            SelectedSlotChanged?.Invoke();
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

        /// <summary>Indica si hay al menos una unidad del ítem en cualquier ranura.</summary>
        public bool HasItem(InventoryItemId itemId)
        {
            if (itemId == InventoryItemId.undefined)
            {
                return false;
            }

            EnsureInitialized();
            for (int i = 0; i < slots.Length; i++)
            {
                InventoryItemDefinition def = slots[i];
                if (def != null && def.Id == itemId)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Quita la primera unidad encontrada del ítem y devuelve true si se quitó alguna.</summary>
        public bool TryRemoveFirstWithItemId(InventoryItemId itemId)
        {
            if (itemId == InventoryItemId.undefined)
            {
                return false;
            }

            EnsureInitialized();
            for (int i = 0; i < slots.Length; i++)
            {
                InventoryItemDefinition def = slots[i];
                if (def == null || def.Id != itemId)
                {
                    continue;
                }

                slots[i] = null;
                InventoryChanged?.Invoke();
                if (selectedSlotIndex == i)
                {
                    SelectedSlotChanged?.Invoke();
                }

                return true;
            }

            return false;
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
            if (selectedSlotIndex >= slots.Length)
            {
                selectedSlotIndex = -1;
            }
        }
    }
}
