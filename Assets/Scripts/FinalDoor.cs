using System;
using StorageEscape.Inventory;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

/// <summary>
/// Puerta final con cuatro <see cref="DoorSlot"/> (llaves azul, roja, verde y morada — <see cref="InventoryItemId.key_purple"/>).
/// Cuando en todas la llave colocada es la correcta (<see cref="DoorSlot.IsCorrect"/>), dispara <see cref="onAllKeysPlaced"/>.
/// </summary>
public class FinalDoor : MonoBehaviour
{
    public const int KeySlotCount = 4;

    [SerializeField] private DoorSlot[] keySlots = new DoorSlot[KeySlotCount];

    [Header("Prefabs llave en ranura (azul / roja / verde / morada)")]
    [SerializeField] private GameObject placedKeyPrefabBlue;
    [SerializeField] private GameObject placedKeyPrefabRed;
    [SerializeField] private GameObject placedKeyPrefabGreen;
    [SerializeField, FormerlySerializedAs("placedKeyPrefabCyan")]
    private GameObject placedKeyPrefabPurple;

    [SerializeField] private UnityEvent onAllKeysPlaced;

    private bool hasRaisedComplete;

    /// <summary>True si en las cuatro ranuras la llave colocada es la correcta.</summary>
    public bool AllKeysPlaced { get; private set; }

    /// <summary>Se dispara una sola vez cuando <see cref="AllKeysPlaced"/> pasa a true.</summary>
    public event Action OnAllKeysPlaced;

    public bool CanOpen => AllKeysPlaced;

    /// <summary>Prefab para mostrar esa llave en ranura; null si no hay en esta puerta (HeldViewPrefab del ítem como respaldo).</summary>
    public GameObject GetPlacedPrefabForKey(InventoryItemId keyId)
    {
        switch (keyId)
        {
            case InventoryItemId.key_blue:
                return placedKeyPrefabBlue;
            case InventoryItemId.key_red:
                return placedKeyPrefabRed;
            case InventoryItemId.key_green:
                return placedKeyPrefabGreen;
            case InventoryItemId.key_purple:
                return placedKeyPrefabPurple;
            default:
                return null;
        }
    }

    private void OnEnable()
    {
        foreach (DoorSlot slot in keySlots)
        {
            if (slot != null)
            {
                slot.SlotStateChanged += OnSlotStateChanged;
            }
        }

        RefreshState();
    }

    private void OnDisable()
    {
        foreach (DoorSlot slot in keySlots)
        {
            if (slot != null)
            {
                slot.SlotStateChanged -= OnSlotStateChanged;
            }
        }
    }

    private void OnSlotStateChanged(DoorSlot _)
    {
        RefreshState();
    }

    private void RefreshState()
    {
        if (keySlots == null || keySlots.Length == 0)
        {
            AllKeysPlaced = false;
            return;
        }

        bool all = true;
        for (int i = 0; i < keySlots.Length; i++)
        {
            DoorSlot slot = keySlots[i];
            if (slot == null || !slot.IsCorrect)
            {
                all = false;
                break;
            }
        }

        AllKeysPlaced = all;

        if (AllKeysPlaced && !hasRaisedComplete)
        {
            hasRaisedComplete = true;
            onAllKeysPlaced?.Invoke();
            OnAllKeysPlaced?.Invoke();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (keySlots == null)
        {
            keySlots = new DoorSlot[KeySlotCount];
        }
        else if (keySlots.Length != KeySlotCount)
        {
            Array.Resize(ref keySlots, KeySlotCount);
        }
    }
#endif
}
