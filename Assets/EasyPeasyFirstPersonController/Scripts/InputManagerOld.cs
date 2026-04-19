namespace EasyPeasyFirstPersonController
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class InputManagerOld : MonoBehaviour, IInputManager
    {
        public Vector2 moveInput
        {
            get
            {
                Keyboard k = Keyboard.current;
                if (k == null)
                {
                    return Vector2.zero;
                }

                float x = 0f;
                float y = 0f;

                if (k.aKey.isPressed || k.leftArrowKey.isPressed)
                {
                    x -= 1f;
                }

                if (k.dKey.isPressed || k.rightArrowKey.isPressed)
                {
                    x += 1f;
                }

                if (k.wKey.isPressed || k.upArrowKey.isPressed)
                {
                    y += 1f;
                }

                if (k.sKey.isPressed || k.downArrowKey.isPressed)
                {
                    y -= 1f;
                }

                Vector2 move = new Vector2(x, y);
                return move.sqrMagnitude > 1f ? move.normalized : move;
            }
        }

        public Vector2 lookInput
        {
            get
            {
                Mouse m = Mouse.current;
                if (m == null)
                {
                    return Vector2.zero;
                }

                return m.delta.ReadValue();
            }
        }

        public bool jump
        {
            get
            {
                Keyboard k = Keyboard.current;
                return k != null && k.spaceKey.isPressed;
            }
        }

        public bool sprint
        {
            get
            {
                Keyboard k = Keyboard.current;
                return k != null && k.leftShiftKey.isPressed;
            }
        }

        public bool crouch
        {
            get
            {
                Keyboard k = Keyboard.current;
                return k != null && k.leftCtrlKey.isPressed;
            }
        }

        public bool slide
        {
            get
            {
                Keyboard k = Keyboard.current;
                return k != null && k.leftCtrlKey.isPressed;
            }
        }
    }
}
