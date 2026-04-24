using System.Collections;
using StorageEscape.Audio;
using StorageEscape.Interaction;
using StorageEscape.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StorageEscape.Puzzles
{
    public class ClockPuzzleManager : MonoBehaviour, IInteractable
    {
        [Header("Inventario")]
        [SerializeField] private InventoryItemId requiredMinuteHandItem = InventoryItemId.minute_hand;

        [Header("Manecillas")]
        [Tooltip("El horario ya debe estar visible desde el inicio.")]
        [SerializeField] private ClockPuzzleHand hourHand;

        [Tooltip("El minutero debe estar apagado al inicio. Se activa al usar el ítem.")]
        [SerializeField] private ClockPuzzleHand minuteHand;

        [Header("UI Mensajes")]
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private float messageDuration = 2f;

        [Header("UI Controles")]
        [SerializeField] private TextMeshProUGUI controlsHudText;

        [Header("Mensajes")]
        [SerializeField] private string missingMinuteHandMessage = "Creo que esto necesita algo más.";
        [SerializeField] private string completeClockMessage = "Ok, ahora sí está completo.";
        [SerializeField] private string controlsMessage = "Q/R: seleccionar manecilla | A/D: girar | ESC: salir";

        [Header("Controles del puzzle")]
        [SerializeField] private Key selectPreviousKey = Key.Q;
        [SerializeField] private Key selectNextKey = Key.R;
        [SerializeField] private Key rotateLeftKey = Key.A;
        [SerializeField] private Key rotateRightKey = Key.D;
        [SerializeField] private Key exitPuzzleKey = Key.Escape;

        [Header("Compartimiento")]
        [SerializeField] private Transform compartmentDoor;
        [SerializeField] private Vector3 openOffset = new Vector3(0f, -1f, 0f);
        [SerializeField] private float openSpeed = 2f;

        [Header("Audio")]
        [SerializeField] private AudioClipId installHandSound = AudioClipId.None;
        [SerializeField] private AudioClipId successSound = AudioClipId.None;

        [Header("Texto de interacción")]
        [SerializeField] private string interactionPrompt = "Usar reloj";

        public string InteractionPrompt => interactionPrompt;

        private ClockPuzzleHand[] hands;

        private bool minuteHandInstalled;
        private bool puzzleActive;
        private bool puzzleSolved;
        private bool opening;

        private int selectedHandIndex;
        private Coroutine messageRoutine;

        private Vector3 closedPosition;
        private Vector3 openPosition;

        private void Start()
        {
            hands = new ClockPuzzleHand[]
            {
                hourHand,
                minuteHand
            };

            if (hourHand != null)
            {
                hourHand.gameObject.SetActive(true);
                hourHand.SetSelected(false);
            }

            if (minuteHand != null)
            {
                minuteHand.gameObject.SetActive(false);
            }

            if (messageText != null)
            {
                messageText.text = string.Empty;
                messageText.gameObject.SetActive(false);
            }

            if (controlsHudText != null)
            {
                controlsHudText.text = controlsMessage;
                controlsHudText.gameObject.SetActive(false);
            }

            if (compartmentDoor != null)
            {
                closedPosition = compartmentDoor.position;
                openPosition = closedPosition + openOffset;
            }

            ClearSelection();
        }

        private void Update()
        {
            HandleOpening();

            if (!puzzleActive || puzzleSolved)
                return;

            Keyboard keyboard = Keyboard.current;

            if (keyboard == null)
                return;

            if (keyboard[exitPuzzleKey].wasPressedThisFrame)
            {
                DeactivatePuzzle();
                return;
            }

            if (keyboard[selectPreviousKey].wasPressedThisFrame)
            {
                SelectPreviousHand();
            }

            if (keyboard[selectNextKey].wasPressedThisFrame)
            {
                SelectNextHand();
            }

            ClockPuzzleHand selectedHand = GetSelectedHand();

            if (selectedHand == null)
                return;

            if (keyboard[rotateLeftKey].wasPressedThisFrame)
            {
                selectedHand.RotateLeft();
                CheckPuzzle();
            }

            if (keyboard[rotateRightKey].wasPressedThisFrame)
            {
                selectedHand.RotateRight();
                CheckPuzzle();
            }
        }

        public bool CanInteract(GameObject interactor)
        {
            return !puzzleSolved;
        }

        public void Interact(GameObject interactor)
        {
            Debug.Log("Interactuando con el reloj.");

            if (puzzleSolved)
                return;

            if (!minuteHandInstalled)
            {
                TryInstallMinuteHand(interactor);
                return;
            }

            ShowMessage(completeClockMessage);
            ActivatePuzzle();
        }

        private void TryInstallMinuteHand(GameObject interactor)
        {
            PlayerInventory inventory = FindInventory(interactor);

            if (inventory == null)
            {
                ShowMessage("No se encontró el inventario del jugador.");
                return;
            }

            if (!inventory.HasItem(requiredMinuteHandItem))
            {
                ShowMessage(missingMinuteHandMessage);
                return;
            }

            bool removedItem = inventory.TryRemoveFirstWithItemId(requiredMinuteHandItem);

            if (!removedItem)
            {
                ShowMessage(missingMinuteHandMessage);
                return;
            }

            minuteHandInstalled = true;

            if (minuteHand != null)
            {
                minuteHand.gameObject.SetActive(true);
                minuteHand.SetSelected(false);
            }

            if (installHandSound != AudioClipId.None && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClip(installHandSound, transform.position, false, true);
            }

            ShowMessage(completeClockMessage);
            ActivatePuzzle();
        }

        private void ActivatePuzzle()
        {
            if (!minuteHandInstalled || puzzleSolved)
                return;

            puzzleActive = true;

            if (controlsHudText != null)
            {
                controlsHudText.text = controlsMessage;
                controlsHudText.gameObject.SetActive(true);
            }

            selectedHandIndex = Mathf.Clamp(selectedHandIndex, 0, hands.Length - 1);
            SelectHand(selectedHandIndex);
        }

        private void DeactivatePuzzle()
        {
            puzzleActive = false;
            ClearSelection();

            if (controlsHudText != null)
            {
                controlsHudText.gameObject.SetActive(false);
            }

            ShowMessage("Dejaste de usar el reloj.");
        }

        private void SelectPreviousHand()
        {
            if (hands == null || hands.Length == 0)
                return;

            selectedHandIndex--;

            if (selectedHandIndex < 0)
            {
                selectedHandIndex = hands.Length - 1;
            }

            SelectHand(selectedHandIndex);
        }

        private void SelectNextHand()
        {
            if (hands == null || hands.Length == 0)
                return;

            selectedHandIndex++;

            if (selectedHandIndex >= hands.Length)
            {
                selectedHandIndex = 0;
            }

            SelectHand(selectedHandIndex);
        }

        private void SelectHand(int index)
        {
            ClearSelection();

            if (hands == null || hands.Length == 0)
                return;

            if (index < 0 || index >= hands.Length)
                return;

            if (hands[index] != null && hands[index].gameObject.activeInHierarchy)
            {
                hands[index].SetSelected(true);
            }
        }

        private void ClearSelection()
        {
            if (hands == null)
                return;

            for (int i = 0; i < hands.Length; i++)
            {
                if (hands[i] != null)
                {
                    hands[i].SetSelected(false);
                }
            }
        }

        private ClockPuzzleHand GetSelectedHand()
        {
            if (hands == null || hands.Length == 0)
                return null;

            if (selectedHandIndex < 0 || selectedHandIndex >= hands.Length)
                return null;

            ClockPuzzleHand hand = hands[selectedHandIndex];

            if (hand == null || !hand.gameObject.activeInHierarchy)
                return null;

            return hand;
        }

        private void CheckPuzzle()
        {
            if (puzzleSolved)
                return;

            if (hourHand == null || minuteHand == null)
                return;

            if (!hourHand.IsCorrect)
                return;

            if (!minuteHand.IsCorrect)
                return;

            SolvePuzzle();
        }

        private void SolvePuzzle()
        {
            puzzleSolved = true;
            puzzleActive = false;
            opening = true;

            ClearSelection();

            if (controlsHudText != null)
            {
                controlsHudText.gameObject.SetActive(false);
            }

            if (hourHand != null)
            {
                hourHand.Lock();
            }

            if (minuteHand != null)
            {
                minuteHand.Lock();
            }

            if (successSound != AudioClipId.None && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClip(successSound, transform.position, false, false);
            }

            ShowMessage("Escuchaste un mecanismo abrirse.");
        }

        private void HandleOpening()
        {
            if (!opening || compartmentDoor == null)
                return;

            compartmentDoor.position = Vector3.Lerp(
                compartmentDoor.position,
                openPosition,
                Time.deltaTime * openSpeed
            );

            if (Vector3.Distance(compartmentDoor.position, openPosition) < 0.01f)
            {
                compartmentDoor.position = openPosition;
                opening = false;
            }
        }

        private void ShowMessage(string message)
        {
            if (messageText == null)
            {
                Debug.LogWarning("ClockPuzzleManager no tiene asignado Message Text.");
                return;
            }

            if (messageRoutine != null)
            {
                StopCoroutine(messageRoutine);
            }

            messageRoutine = StartCoroutine(MessageRoutine(message));
        }

        private IEnumerator MessageRoutine(string message)
        {
            messageText.text = message;
            messageText.gameObject.SetActive(true);

            yield return new WaitForSeconds(messageDuration);

            messageText.gameObject.SetActive(false);
        }

        private static PlayerInventory FindInventory(GameObject interactor)
        {
            if (interactor == null)
                return null;

            return interactor.GetComponent<PlayerInventory>()
                ?? interactor.GetComponentInParent<PlayerInventory>()
                ?? interactor.GetComponentInChildren<PlayerInventory>();
        }
    }
}