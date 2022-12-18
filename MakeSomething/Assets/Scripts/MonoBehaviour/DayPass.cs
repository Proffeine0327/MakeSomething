using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayPass : MonoBehaviour
{
    private static DayPass daypass;

    /// <summary>
    /// Set sun and moon group rotation, and background color.
    /// </summary>
    /// <param name="t">Time to set. 1t == 1min(1/3600day)</param>
    public static void SetTime(float t) => daypass.time = t;

    [SerializeField] private float time;
    [SerializeField] private GameObject sun;
    [SerializeField] private GameObject moon;
    [Header("sky")]
    [SerializeField] private SpriteRenderer sky;
    [SerializeField] private Color daySkyColor;
    [SerializeField] private Color nightfallSkyColor;
    [SerializeField] private Color nightSkyColor;
    [Header("globalLight")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private Color dayLightColor;
    [SerializeField] private Color nightfallLightColor;
    [SerializeField] private Color nightLightColor;

    private void Awake() 
    {
        daypass = this;
    }

    void Update()
    {
        sun.transform.rotation = Quaternion.Euler(0,0,0);
        moon.transform.rotation = Quaternion.Euler(0,0,0);

        daypass.transform.rotation = Quaternion.Euler(0,0,-(time % 3600)/10);

        #region light
        if((time % 3600) is >= 0 and <= 200)
        {
            globalLight.color = dayLightColor;
            globalLight.intensity = 0.3f;
        }

        if((time % 3600) is >= 200 and <= 600)
        {
            globalLight.color = dayLightColor;
            globalLight.intensity = Mathf.Lerp(0.3f, 1.3f, ((time % 3600) - 200) / 400);
        }

        if((time % 3600) is >= 600 and <= 1200)
        {
            globalLight.color = dayLightColor;
            globalLight.intensity = 1.3f;
        }

        if((time % 3600) is >= 1200 and <= 1600)
        {
            globalLight.color = Color.Lerp(dayLightColor, nightfallLightColor, ((time % 3600) - 1200) / 400);
            globalLight.intensity = Mathf.Lerp(1.3f, 0.9f, ((time % 3600) - 1200) / 400);
        }
        
        if((time % 3600) is >= 1600 and <= 1650)
        {
            globalLight.color = nightfallLightColor;
            globalLight.intensity = 0.9f;   
        }

        if((time % 3600) is >= 1650 and <= 1900)
        {
            globalLight.color = Color.Lerp(nightfallLightColor, nightLightColor, (((time % 3600) % 3600) - 1650) / 250);
            globalLight.intensity = Mathf.Lerp(0.9f, 0.3f, ((time % 3600) - 1650) / 250);
        }

        if((time % 3600) is >= 1900 and <= 3400)
        {
            globalLight.color = nightLightColor;
            globalLight.intensity = 0.3f;
        }

        if((time % 3600) is >= 3400 and <= 3600)
        {
            globalLight.color = Color.Lerp(nightLightColor, dayLightColor, ((time % 3600) - 3400) / 200);
            globalLight.intensity = 0.3f;
        }
        #endregion

        #region sky
        if((time % 3600) is >= 0 and <= 1200)
        {
            sky.color = daySkyColor;
        }

        if((time % 3600) is >= 1200 and <= 1600)
        {
            sky.color = Color.Lerp(daySkyColor, nightfallSkyColor, ((time % 3600) - 1200) / 400);
        }

        if((time % 3600) is >= 1600 and <= 1650)
        {
            sky.color = nightfallSkyColor;
        }

        if((time % 3600) is >= 1650 and <= 1900)
        {
            sky.color = Color.Lerp(nightfallSkyColor, nightSkyColor, ((time % 3600) - 1650) / 250);
        }

        if((time % 3600) is >= 1900 and <= 3400)
        {
            sky.color = nightSkyColor;
        }

        if((time % 3600) is >= 3400 and <= 3600)
        {
            sky.color = Color.Lerp(nightSkyColor, daySkyColor, ((time % 3600) - 3400) / 200);
        }
        #endregion
    }
}
