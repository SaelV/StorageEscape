using System.Collections;
using UnityEngine;

namespace StorageEscape.Interaction
{
    /// <summary>
    /// Primera interacción: activa el objetivo durante un tiempo breve y luego lo apaga.
    /// Segunda interacción: lo deja activo de forma permanente y deshabilita más interacciones.
    /// </summary>
    public class InteractablePulseThenPermanent : MonoBehaviour, IInteractable
    {
        private enum Phase
        {
            AwaitingFirst,
            PulsingFirst,
            AwaitingSecond,
            Completed
        }

        [SerializeField] private string interactionPrompt = "Interactuar";
        [SerializeField] private string interactionPromptSecond = "Interactuar";
        [SerializeField] private GameObject target;
        [SerializeField] private float firstPulseDurationSeconds = 0.2f;

        private Phase phase = Phase.AwaitingFirst;

        public string InteractionPrompt =>
            phase == Phase.AwaitingSecond ? interactionPromptSecond : interactionPrompt;

        public bool CanInteract(GameObject interactor)
        {
            return phase is Phase.AwaitingFirst or Phase.AwaitingSecond;
        }

        public void Interact(GameObject interactor)
        {
            switch (phase)
            {
                case Phase.AwaitingFirst:
                    if (target != null)
                    {
                        target.SetActive(true);
                    }

                    phase = Phase.PulsingFirst;
                    StartCoroutine(FirstPulseRoutine());
                    break;

                case Phase.AwaitingSecond:
                    if (target != null)
                    {
                        target.SetActive(true);
                    }

                    phase = Phase.Completed;
                    break;
            }
        }

        private IEnumerator FirstPulseRoutine()
        {
            yield return new WaitForSeconds(firstPulseDurationSeconds);

            if (phase == Phase.PulsingFirst)
            {
                if (target != null)
                {
                    target.SetActive(false);
                }

                phase = Phase.AwaitingSecond;
            }
        }
    }
}
