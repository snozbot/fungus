using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using UnityEngine.UI;

namespace TimeScaleDemo
{
    public class TimeScaleDemoScript : MonoBehaviour
    {
        public float rotationSpeed = 20f;
        public Button timePauseButton;
        private Text timePauseButtonText;
        public Button timeScaleButton;
        private Text timeScaleButtonText;

        void Start()
        {
            FungusManager.Instance.useUnscaledTime = true;

            timePauseButtonText = timePauseButton.GetComponentInChildren<Text>();
            UnityEngine.Events.UnityAction pauseAction = delegate { TogglePause(); };
            timePauseButton.onClick.AddListener(pauseAction);

            timeScaleButtonText = timeScaleButton.GetComponentInChildren<Text>();
            UnityEngine.Events.UnityAction timeScaleAction = delegate { ToggleTimeScale(); };
            timeScaleButton.onClick.AddListener(timeScaleAction);
        }

        void Update()
        {
            transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0), Space.World);
        }

        public void TogglePause()
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;

            if (Time.timeScale == 0)
            {
                timePauseButtonText.text = "Resume Time";
            }
            else
            {
                timePauseButtonText.text = "Pause Time";
            }
        }

        public void ToggleTimeScale()
        {
            FungusManager.Instance.useUnscaledTime = !FungusManager.Instance.useUnscaledTime;

            if (FungusManager.Instance.useUnscaledTime)
            {
                timeScaleButtonText.text = "Use Scaled Time";
                if (Time.timeScale == 0) TogglePause();
            }
            else
            {
                timeScaleButtonText.text = "Use Unscaled Time";
            }
        }
    }
}
