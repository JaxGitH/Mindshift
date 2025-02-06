using UnityEngine;

namespace Lightbug.CharacterControllerPro.Implementation
{
    /// <summary>
    /// Defines the nature of the inputs obtained by the associated input handler.
    /// </summary>
    public enum HumanInputType
    {
        InputManager,
        UIMobile,
        Custom
    }

    /// <summary>
    /// This abstract class contains all the input methods that are used by the character brain. This is the base class for all the input detection methods available.
    /// </summary>
    public abstract class InputHandler : MonoBehaviour
    {
        public static InputHandler CreateInputHandler(GameObject gameObject, HumanInputType inputType)
        {
            InputHandler inputHandler = null;

            switch (inputType)
            {
                case HumanInputType.InputManager:

                    inputHandler = gameObject.AddComponent<UnityInputHandler>();

                    break;
                case HumanInputType.UIMobile:

                    inputHandler = gameObject.AddComponent<UIInputHandler>();

                    break;
            }

            return inputHandler;
        }

        public abstract bool GetBool(string actionName);
        public abstract float GetFloat(string actionName);
        public abstract Vector2 GetVector2(string actionName);
    }
}
