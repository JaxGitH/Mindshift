using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Lightbug.CharacterControllerPro.Demo
{
    public class MenuButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        string sceneName = "";

        [SerializeField]
        Color highlightColor = Color.green;

        [SerializeField]
        float lerpSpeed = 5f;

        Color normalColor;

        Image image = null;

        bool enter = false;

        void Awake()
        {
            image = GetComponent<Image>();

            if (image == null)
            {
                enabled = false;
                return;
            }

            normalColor = image.color;
        }

        void Update()
        {
            image.color = Color.Lerp(image.color, enter ? highlightColor : normalColor, lerpSpeed * Time.deltaTime);

            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            MainMenuManager.Instance.GoToScene(sceneName);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            enter = true;        
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            enter = false;
        }
    }

}
