using UnityEngine;
using UnityEngine.InputSystem;

namespace StorageEscape.Interaction
{
    /// <summary>
    /// Detecta un <see cref="IInteractable"/> frente al jugador y dispara <see cref="IInteractable.Interact"/> al pulsar la tecla.
    /// Añádelo al mismo GameObject que el controlador en primera persona y asigna la cámara del jugador.
    /// </summary>
    public class PlayerInteractionController : MonoBehaviour
    {
        [SerializeField] private Transform viewTransform;
        [SerializeField] private float maxDistance = 3f;
        [SerializeField] private LayerMask raycastMask = ~0;
        [SerializeField] private Key interactKey = Key.E;
        [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide;

        public IInteractable CurrentTarget { get; private set; }

        private void Reset()
        {
            var cam = GetComponentInChildren<Camera>();
            if (cam != null)
            {
                viewTransform = cam.transform;
            }
        }

        private void Update()
        {
            RefreshTarget();

            if (CurrentTarget == null)
            {
                return;
            }

            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (!keyboard[interactKey].wasPressedThisFrame)
            {
                return;
            }

            if (!CurrentTarget.CanInteract(gameObject))
            {
                return;
            }

            CurrentTarget.Interact(gameObject);
        }

        private void RefreshTarget()
        {
            if (viewTransform == null)
            {
                CurrentTarget = null;
                return;
            }

            Ray ray = new Ray(viewTransform.position, viewTransform.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, maxDistance, raycastMask, triggerInteraction))
            {
                CurrentTarget = null;
                return;
            }

            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            CurrentTarget = interactable;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (viewTransform == null)
            {
                return;
            }

            Gizmos.color = CurrentTarget != null ? Color.green : Color.cyan;
            Vector3 end = viewTransform.position + viewTransform.forward * maxDistance;
            Gizmos.DrawLine(viewTransform.position, end);
        }
#endif
    }
}
