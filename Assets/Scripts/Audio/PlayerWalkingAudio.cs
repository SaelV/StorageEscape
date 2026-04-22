using EasyPeasyFirstPersonController;
using UnityEngine;

namespace StorageEscape.Audio
{
    /// <summary>
    /// Loop de <see cref="AudioClipId.PlayerWalking"/> mientras hay input de movimiento en <see cref="FirstPersonController"/>;
    /// se detiene al soltar las teclas de movimiento.
    /// </summary>
    [RequireComponent(typeof(FirstPersonController))]
    public class PlayerWalkingAudio : MonoBehaviour
    {
        [SerializeField] private AudioClipId clipId = AudioClipId.PlayerWalking;

        private FirstPersonController fps;
        private AudioSource walkingSource;

        private void Awake()
        {
            fps = GetComponent<FirstPersonController>();
            var go = new GameObject("WalkingLoopAudio");
            go.transform.SetParent(transform, false);
            walkingSource = go.AddComponent<AudioSource>();
            walkingSource.playOnAwake = false;
        }

        private void OnDestroy()
        {
            if (walkingSource != null)
            {
                walkingSource.Stop();
            }
        }

        private void LateUpdate()
        {
            if (AudioManager.Instance == null || fps == null || fps.input == null)
            {
                StopLoop();
                return;
            }

            bool moveHeld = fps.input.moveInput.sqrMagnitude > 0.0001f;
            bool shouldPlay = moveHeld && fps.isGrounded;

            if (!shouldPlay)
            {
                StopLoop();
                return;
            }

            if (walkingSource.isPlaying)
            {
                return;
            }

            if (!AudioManager.Instance.TryConfigureSourceFromCatalog(clipId, walkingSource, loop: true, randomizePitch: false))
            {
                return;
            }

            walkingSource.Play();
        }

        private void StopLoop()
        {
            if (walkingSource == null || !walkingSource.isPlaying)
            {
                return;
            }

            walkingSource.Stop();
            walkingSource.clip = null;
            walkingSource.loop = false;
        }
    }
}
