using StorageEscape.Audio;
using System.Collections;
using UnityEngine;

namespace StorageEscape.Interaction
{
    /// <summary>
    /// Al interactuar mueve un objeto hasta la pose de <see cref="openTarget"/>; la siguiente interacción vuelve a la pose cerrada.
    /// Pensado para cajones, puertas u objetos que basculan o se desplazan.
    /// Opcionalmente puede activar un <see cref="GameObject"/> al abrir y desactivarlo al cerrar.
    /// </summary>
    public class InteractableToggleMove : MonoBehaviour, IInteractable
    {
        [Header("Objetivo")]
        [SerializeField] private Transform objectToMove;
        [SerializeField] private Transform openTarget;

        [Header("Pose cerrada")]
        [Tooltip("Si está vacío, la pose cerrada es la del objeto al iniciar la escena.")]
        [SerializeField] private Transform closedReference;

        [Header("Animación")]
        [SerializeField] private float moveDurationSeconds = 0.5f;
        [SerializeField] private AnimationCurve easing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Texto UI")]
        [SerializeField] private string promptWhenClosed = "Abrir";
        [SerializeField] private string promptWhenOpen = "Cerrar";

        [Header("Opcional")]
        [Tooltip("Se activa al empezar a abrir y se desactiva al empezar a cerrar.")]
        [SerializeField] private GameObject activeWhenOpening;
        [SerializeField] private AudioClipId audioClipId = AudioClipId.Switch;

        private Vector3 closedWorldPosition;
        private Quaternion closedWorldRotation;
        private bool isAtOpenPose;
        private bool isMoving;

        private void Awake()
        {
            if (objectToMove == null)
            {
                objectToMove = transform;
            }
        }

        private void Start()
        {
            if (closedReference != null)
            {
                closedWorldPosition = closedReference.position;
                closedWorldRotation = closedReference.rotation;
            }
            else
            {
                closedWorldPosition = objectToMove.position;
                closedWorldRotation = objectToMove.rotation;
            }

            if (openTarget != null && PoseMatches(objectToMove.position, objectToMove.rotation, openTarget.position, openTarget.rotation))
            {
                isAtOpenPose = true;
            }

            SetLinkedObjectForTransition(isAtOpenPose);
        }

        public string InteractionPrompt => isAtOpenPose ? promptWhenOpen : promptWhenClosed;

        public bool CanInteract(GameObject interactor) => !isMoving;

        public void Interact(GameObject interactor)
        {
            if (openTarget == null || objectToMove == null || isMoving)
            {
                return;
            }

            if (moveDurationSeconds <= 0f)
            {
                SetLinkedObjectForTransition(!isAtOpenPose);
                ApplyPoseInstant(isAtOpenPose ? closedWorldPosition : openTarget.position, isAtOpenPose ? closedWorldRotation : openTarget.rotation);
                isAtOpenPose = !isAtOpenPose;
                return;
            }
            AudioManager.Instance.PlayClip(audioClipId, objectToMove.position);
            StartCoroutine(MoveRoutine());
        }

        private IEnumerator MoveRoutine()
        {
            isMoving = true;

            SetLinkedObjectForTransition(!isAtOpenPose);

            Vector3 fromPos = objectToMove.position;
            Quaternion fromRot = objectToMove.rotation;
            Vector3 toPos = isAtOpenPose ? closedWorldPosition : openTarget.position;
            Quaternion toRot = isAtOpenPose ? closedWorldRotation : openTarget.rotation;

            float duration = Mathf.Max(0.0001f, moveDurationSeconds);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = easing != null && easing.length > 0 ? easing.Evaluate(t) : t;
                objectToMove.SetPositionAndRotation(
                    Vector3.LerpUnclamped(fromPos, toPos, eased),
                    Quaternion.SlerpUnclamped(fromRot, toRot, eased));
                yield return null;
            }

            objectToMove.SetPositionAndRotation(toPos, toRot);
            isAtOpenPose = !isAtOpenPose;
            isMoving = false;
        }

        private void ApplyPoseInstant(Vector3 worldPosition, Quaternion worldRotation)
        {
            objectToMove.SetPositionAndRotation(worldPosition, worldRotation);
        }

        private void SetLinkedObjectForTransition(bool movingToOpen)
        {
            if (activeWhenOpening == null)
            {
                return;
            }

            activeWhenOpening.SetActive(movingToOpen);
        }

        private static bool PoseMatches(Vector3 aPos, Quaternion aRot, Vector3 bPos, Quaternion bRot)
        {
            const float posEpsilon = 0.001f;
            const float rotEpsilon = 0.5f;
            return Vector3.SqrMagnitude(aPos - bPos) < posEpsilon * posEpsilon
                && Quaternion.Angle(aRot, bRot) < rotEpsilon;
        }

        private void OnValidate()
        {
            if (moveDurationSeconds < 0f)
            {
                moveDurationSeconds = 0f;
            }

            if (easing == null || easing.length == 0)
            {
                easing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            }
        }
    }
}
