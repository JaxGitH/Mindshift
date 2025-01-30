using UnityEngine;
using UnityEngine.EventSystems;

namespace Lightbug.CharacterControllerPro.Implementation
{
    /// <summary>
    /// This class reads the actions of a 2D UI button and then sends the states flags to a mobile input component.
    /// </summary>
    [AddComponentMenu("Character Controller Pro/Implementation/UI/Input Button")]
    public class InputButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IUIBoolAction
    {
        [SerializeField]
        string actionName = "";

        bool boolValue;

        #region IBoolAction

        public string ActionName => actionName;
        public bool BoolValue => boolValue;

        #endregion

        public void OnPointerDown(PointerEventData eventData) => boolValue = true;
        public void OnPointerUp(PointerEventData eventData) => boolValue = false;
    }
}