using UnityEngine;
using UnityEngine.Events;

namespace StorageEscape.Interaction
{
    /// <summary>
    /// Implementación mínima de <see cref="IInteractable"/> configurable desde el inspector (<see cref="UnityEvent"/>).
    /// </summary>
    public class InteractableUnityEvent : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionPrompt = "Interactuar";
        [SerializeField] private UnityEvent<GameObject> onInteract;

        public string InteractionPrompt => interactionPrompt;

        public bool CanInteract(GameObject interactor) => true;

        public void Interact(GameObject interactor)
        {
            onInteract?.Invoke(interactor);
        }
    }
}
