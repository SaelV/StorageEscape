using StorageEscape.Audio;
using StorageEscape.Interaction;
using StorageEscape.Inventory;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Ranura de puerta: solo acepta la <see cref="correctKey"/> en mano y la instancia bajo <see cref="placementAnchor"/>.
/// El modelo sale de <see cref="FinalDoor"/> (padre) o del HeldViewPrefab del ítem.
/// </summary>
public class DoorSlot : MonoBehaviour, IInteractable
{
    [Tooltip("Donde se instancia la llave colocada.")]
    [SerializeField, FormerlySerializedAs("keyAnchor")]
    private Transform placementAnchor;

    [Tooltip("Única llave que encaja en esta ranura.")]
    [SerializeField] private InventoryItemId correctKey = InventoryItemId.undefined;

    [SerializeField] private string interactionPrompt = "Colocar llave";

    [SerializeField] private AudioClipId placeKeySound = AudioClipId.PickUpKey;

    private GameObject placedVisual;
    private bool keyPlaced;
    private InventoryItemId placedKeyId = InventoryItemId.undefined;

    public bool IsOccupied => keyPlaced;

    public bool IsCorrect => keyPlaced && placedKeyId == correctKey && correctKey != InventoryItemId.undefined;

    public InventoryItemId ExpectedKey => correctKey;

    public InventoryItemId PlacedKeyId => placedKeyId;

    public event System.Action<DoorSlot> SlotStateChanged;

    public string InteractionPrompt => interactionPrompt;

    public bool CanInteract(GameObject interactor)
    {
        if (keyPlaced || correctKey == InventoryItemId.undefined || placementAnchor == null)
        {
            return false;
        }

        PlayerInventory inventory = interactor.GetComponentInParent<PlayerInventory>();
        if (inventory == null)
        {
            return false;
        }

        InventoryItemDefinition held = inventory.HeldItem;
        if (held == null || held.Id != correctKey)
        {
            return false;
        }

        return GetSpawnPrefab(held) != null;
    }

    public void Interact(GameObject interactor)
    {
        if (!CanInteract(interactor))
        {
            return;
        }

        PlayerInventory inventory = interactor.GetComponentInParent<PlayerInventory>();
        if (inventory == null)
        {
            return;
        }

        InventoryItemDefinition held = inventory.HeldItem;
        if (held == null || !inventory.TryConsumeHeldItem(correctKey))
        {
            return;
        }

        keyPlaced = true;
        placedKeyId = correctKey;

        SpawnPlacedVisual(held);

        if (AudioManager.Instance != null && placeKeySound != AudioClipId.None)
        {
            AudioManager.Instance.PlayClip(placeKeySound, placementAnchor.position);
        }

        SlotStateChanged?.Invoke(this);
    }

    private GameObject GetSpawnPrefab(InventoryItemDefinition definition)
    {
        FinalDoor door = GetComponentInParent<FinalDoor>();
        if (door != null)
        {
            GameObject fromDoor = door.GetPlacedPrefabForKey(correctKey);
            if (fromDoor != null)
            {
                return fromDoor;
            }
        }

        return definition != null ? definition.HeldViewPrefab : null;
    }

    private void SpawnPlacedVisual(InventoryItemDefinition definition)
    {
        if (placedVisual != null)
        {
            Destroy(placedVisual);
            placedVisual = null;
        }

        GameObject prefab = GetSpawnPrefab(definition);
        if (prefab == null || placementAnchor == null)
        {
            Debug.LogWarning(
                $"[DoorSlot] '{name}': sin prefab para '{correctKey}' (FinalDoor padre o HeldViewPrefab en '{definition.name}').",
                this);
            return;
        }

        placedVisual = Instantiate(prefab, placementAnchor);
        placedVisual.transform.localPosition = Vector3.zero;
        placedVisual.transform.localRotation = Quaternion.identity;
        placedVisual.transform.localScale = Vector3.one;
    }

    private void OnDestroy()
    {
        if (placedVisual != null)
        {
            Destroy(placedVisual);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (placementAnchor == null)
        {
            Transform child = transform.Find("PlacementAnchor") ?? transform.Find("KeyAnchor");
            if (child != null)
            {
                placementAnchor = child;
            }
        }
    }
#endif
}
