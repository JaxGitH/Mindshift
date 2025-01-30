using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lightbug.Utilities
{

    public class FpsCounter : MonoBehaviour
    {

        [SerializeField]
        float refreshTime = 0.2f;

        [SerializeField]
        Text text = null;

        [SerializeField]
        bool limitToRefreshRate = true;


        public float Fps => fps;

        int samples = 0;
        string output = "FPS : ";
        float fps = 60f;
        Dictionary<float, string> frames = new Dictionary<float, string>();

        float GetRefreshRateValue()
        {
#if UNITY_2023_1_OR_NEWER
            return (float)Screen.currentResolution.refreshRateRatio.value;
#else
            return Screen.currentResolution.refreshRate;
#endif
        }

        void Awake()
        {
            fps = GetRefreshRateValue();

            // Max value = 1000.00
            // Resolution = 0.01
            for (int i = 0; i < 100000; i++)
            {
                float frameFloat = i / 100f;
                frames.Add(i, frameFloat.ToString("F2"));
            }

            if (text != null)
                StartCoroutine(UpdateFPS());
        }


        float time = 0f;

        void Update()
        {
            time += Time.unscaledDeltaTime;
            samples++;

            if (time >= refreshTime)
            {
                fps = samples / time;
                PrintData();

                time -= refreshTime;
                samples = 0;
            }
        }

        IEnumerator UpdateFPS()
        {
            var waitInstruction = new WaitForSecondsRealtime(refreshTime);
            while (true)
            {
                yield return waitInstruction;
                PrintData();
            }
        }

        private void PrintData()
        {
            if (limitToRefreshRate && QualitySettings.vSyncCount != 0)
                fps = Mathf.Min(fps, GetRefreshRateValue());
            else
                fps = Mathf.Min(fps, 1000f);

            output = frames[(int)(fps * 100)];
            text.text = $"{output}\n time = {(1000f * time / samples)} ms";
        }
    }

}
