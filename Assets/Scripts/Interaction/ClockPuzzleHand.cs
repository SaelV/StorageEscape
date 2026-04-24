using StorageEscape.Audio;
using UnityEngine;

namespace StorageEscape.Puzzles
{
    public class ClockPuzzleHand : MonoBehaviour
    {
        [Header("Hora")]
        [SerializeField, Range(1, 12)] private int currentHour = 12;
        [SerializeField, Range(1, 12)] private int correctHour = 3;

        [Header("Rotación")]
        [SerializeField] private float stepAngle = 30f;
        [SerializeField] private Vector3 rotationAxis = Vector3.forward;

        [Header("Highlight")]
        [SerializeField] private Renderer targetRenderer;

        [Tooltip("Material normal de la manecilla.")]
        [SerializeField] private Material normalMaterial;

        [Tooltip("Material visible cuando la manecilla está seleccionada. Recomiendo uno amarillo/emissive.")]
        [SerializeField] private Material selectedMaterial;

        [Tooltip("Opcional: objeto extra de highlight, como una copia amarilla o un outline.")]
        [SerializeField] private GameObject highlightObject;

        [Header("Audio")]
        [SerializeField] private AudioClipId rotateSound = AudioClipId.None;

        private bool locked;

        public bool IsCorrect => currentHour == correctHour;

        private void Awake()
        {
            if (targetRenderer == null)
            {
                targetRenderer = GetComponentInChildren<Renderer>();
            }

            if (targetRenderer != null && normalMaterial == null)
            {
                normalMaterial = targetRenderer.sharedMaterial;
            }

            SetSelected(false);
        }

        public void RotateRight()
        {
            if (locked)
                return;

            currentHour++;

            if (currentHour > 12)
            {
                currentHour = 1;
            }

            transform.Rotate(rotationAxis, -stepAngle, Space.Self);
            PlayRotateSound();
        }

        public void RotateLeft()
        {
            if (locked)
                return;

            currentHour--;

            if (currentHour < 1)
            {
                currentHour = 12;
            }

            transform.Rotate(rotationAxis, stepAngle, Space.Self);
            PlayRotateSound();
        }

        public void SetSelected(bool selected)
        {
            if (targetRenderer != null)
            {
                if (selected && selectedMaterial != null)
                {
                    targetRenderer.material = selectedMaterial;
                }
                else if (!selected && normalMaterial != null)
                {
                    targetRenderer.material = normalMaterial;
                }
            }

            if (highlightObject != null)
            {
                highlightObject.SetActive(selected);
            }
        }

        public void Lock()
        {
            locked = true;
            SetSelected(false);
        }

        private void PlayRotateSound()
        {
            if (rotateSound == AudioClipId.None || AudioManager.Instance == null)
                return;

            AudioManager.Instance.PlayClip(rotateSound, transform.position, false, true);
        }
    }
}