using StorageEscape.Audio;
using UnityEngine;

namespace StorageEscape.Interaction
{
    /// <summary>
    /// Interruptor de luz: al interactuar alterna encendido/apagado.
    /// Encendido: la luz se ilumina y el collider asociado queda activo.
    /// Apagado: la luz se apaga y el collider se desactiva.
    /// </summary>
    public class LightReveal : MonoBehaviour, IInteractable
    {
        [Header("Referencias")]
        [SerializeField] private Light targetLight;
        [Tooltip("Collider (por ejemplo de una zona revelada) que solo debe existir con la luz encendida.")]
        [SerializeField] private Collider linkedCollider;

        [Header("Estado inicial")]
        [SerializeField] private bool startsOn;

        [Header("Texto UI")]
        [SerializeField] private string promptWhenOff = "Encender";
        [SerializeField] private string promptWhenOn = "Apagar";

        private bool isOn;

        public string InteractionPrompt => isOn ? promptWhenOn : promptWhenOff;

        private void Awake()
        {
            isOn = startsOn;
            ApplyVisualState(isOn);
        }

        public bool CanInteract(GameObject interactor) => interactor != null;

        public void Interact(GameObject interactor)
        {
            isOn = !isOn;
            ApplyVisualState(isOn);
            AudioManager.Instance.PlayClip(AudioClipId.Switch, transform.position);
        }

        private void ApplyVisualState(bool on)
        {
            if (targetLight != null)
            {
                targetLight.enabled = on;
            }

            if (linkedCollider != null)
            {
                linkedCollider.enabled = on;
            }
        }
    }
}
