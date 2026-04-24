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
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.yellow;

        [Header("Audio")]
        [SerializeField] private AudioClipId rotateSound = AudioClipId.None;

        private MaterialPropertyBlock propertyBlock;
        private bool locked;

        public bool IsCorrect => currentHour == correctHour;

        private void Awake()
        {
            if (targetRenderer == null)
            {
                targetRenderer = GetComponentInChildren<Renderer>();
            }

            propertyBlock = new MaterialPropertyBlock();
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
            if (targetRenderer == null)
                return;

            targetRenderer.GetPropertyBlock(propertyBlock);

            Color color = selected ? selectedColor : normalColor;

            propertyBlock.SetColor("_BaseColor", color);
            propertyBlock.SetColor("_Color", color);

            targetRenderer.SetPropertyBlock(propertyBlock);
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