using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Lightbug.CharacterControllerPro.Implementation
{
    /// <summary>
    /// This class reads the actions of a 2D UI joystick and then sends the values to a mobile input component.
    /// </summary>
    [AddComponentMenu("Character Controller Pro/Implementation/UI/Input Axes")]
    public class InputAxes : MonoBehaviour, IDragHandler, IEndDragHandler, IUIVector2Action
    {
        public enum DeadZoneMode
        {
            Radial,
            PerAxis
        }

        [Header("Targets")]

        // [SerializeField]
        // MobileInput horizontalAxisMobileInput = null;

        // [SerializeField]
        // MobileInput verticalAxisMobileInput = null;

        [SerializeField]
        string actionName = "";

        [Header("Handles properties")]

        [SerializeField]
        bool invertHorizontal = false;

        [SerializeField]
        bool invertVertical = false;


        [Tooltip("How is the dead zone affected the output value. To visualize better the dead zone, think of \"Radial\" as a circle, and \"PerAxis\" as a cardinal cross.")]
        [SerializeField]
        DeadZoneMode deadZoneMode = DeadZoneMode.Radial;

        [Tooltip("Minimum amount of magnitude (considering the axis scale) needed to produce a non zero output. Magnitudes lower than this value will be considered as zero.")]
        [Range(0f, 1f)]
        [SerializeField]
        float deadZoneDistance = 0.2f;

        [SerializeField]
        int boundsRadius = 50;

        [Header("Handle visuals")]

        [Range(2f, 50f)]
        [SerializeField]
        float returnLerpSpeed = 10f;

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        #region IVector2Action

        public string ActionName => actionName;
        public Vector2 Vector2Value => vector2Value;

        #endregion

        Vector2 vector2Value;
        Vector2 virtualPosition = default(Vector2);
        Vector2 visiblePosition = default(Vector2);

        RectTransform rectTransform = null;

        Vector2 origin = Vector2.zero;

        bool drag = false;

        void Awake()
        {
            virtualPosition = origin;

            rectTransform = GetComponent<RectTransform>();
        }


        void Update()
        {

            // Motion -------------------------------------------------------------------------------------------------     
            if (!drag)
            {
                virtualPosition = visiblePosition;
                virtualPosition = Vector2.Lerp(virtualPosition, origin, returnLerpSpeed * Time.deltaTime);
            }

            Vector2 delta = virtualPosition - origin;


            visiblePosition = origin + Vector2.ClampMagnitude(delta, boundsRadius);


            rectTransform.anchoredPosition = visiblePosition;

            Vector2 axesValue = (visiblePosition - origin) / boundsRadius;

            // Axes ------------------------------------------------------------------------------------------------        

            if (deadZoneMode == DeadZoneMode.Radial)
            {
                float radius = Vector3.Magnitude(axesValue);

                axesValue.x = radius > deadZoneDistance ? axesValue.x : 0f;
                axesValue.y = radius > deadZoneDistance ? axesValue.y : 0f;
            }
            else
            {
                float absX = Mathf.Abs(axesValue.x);
                float absY = Mathf.Abs(axesValue.y);

                axesValue.x = absX > deadZoneDistance ? axesValue.x : 0f;
                axesValue.y = absY > deadZoneDistance ? axesValue.y : 0f;
            }

            if (invertHorizontal)
                axesValue.x *= -1;

            if (invertVertical)
                axesValue.y *= -1;

            //vector2Action.value = axesValue;
            vector2Value = axesValue;
        }

        public void OnDrag(PointerEventData eventData)
        {
            drag = true;

            virtualPosition += eventData.delta / 2f;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            drag = false;
        }
    }
}

