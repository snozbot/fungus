using UnityEngine;
using System.Collections;

public class TimeControl : MonoBehaviour
{

    public void SetTimeScale(float timeScale)
    {
        if (timeScale < 0)
            Time.timeScale = 0;
        else
            Time.timeScale = timeScale;
    }
}
