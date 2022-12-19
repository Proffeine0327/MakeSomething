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
    /// <param name="t">Time to set. 1t == 1sec(1/86400day)</param>
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

        daypass.transform.rotation = Quaternion.Euler(0,0,-(time * 0.00416666667f));

        #region light
        if((time % 86400) is >= 0 and <= 4800)
        {
            globalLight.color = dayLightColor;
            globalLight.intensity = 0.3f;
        }
        else if((time % 86400) is <= 14400)
        {
            globalLight.color = dayLightColor;
            globalLight.intensity = Mathf.Lerp(0.3f, 1.3f, ((time % 86400) - 4800) / 9600);
        }
        else if((time % 86400) is <= 28800)
        {
            globalLight.color = dayLightColor;
            globalLight.intensity = 1.3f;
        }
        else if((time % 86400) is <= 38400)
        {
            globalLight.color = Color.Lerp(dayLightColor, nightfallLightColor, ((time % 86400) - 28800) / 9600);
            globalLight.intensity = Mathf.Lerp(1.3f, 0.9f, ((time % 86400) - 28800) / 9600);
        }
        else if((time % 86400) is <= 39600)
        {
            globalLight.color = nightfallLightColor;
            globalLight.intensity = 0.9f;   
        }
        else if((time % 86400) is <= 45600)
        {
            globalLight.color = Color.Lerp(nightfallLightColor, nightLightColor, (((time % 86400) % 86400) - 39600) / 6000);
            globalLight.intensity = Mathf.Lerp(0.9f, 0.3f, ((time % 86400) - 39600) / 6000);
        }
        else if((time % 86400) <= 81600)
        {
            globalLight.color = nightLightColor;
            globalLight.intensity = 0.3f;
        }
        else if((time % 86400) <= 86400)
        {
            globalLight.color = Color.Lerp(nightLightColor, dayLightColor, ((time % 86400) - 81600) / 4800);
            globalLight.intensity = 0.3f;
        }
        #endregion

        #region sky
        if((time % 86400) is >= 0 and <= 28800)
        {
            sky.color = daySkyColor;
        }
        else if((time % 86400) is <= 38400)
        {
            sky.color = Color.Lerp(daySkyColor, nightfallSkyColor, ((time % 86400) - 28800) / 9600);
        }
        else if((time % 86400) is <= 39600)
        {
            sky.color = nightfallSkyColor;
        }
        else if((time % 86400) is <= 45600)
        {
            sky.color = Color.Lerp(nightfallSkyColor, nightSkyColor, ((time % 86400) - 39600) / 6000);
        }
        else if((time % 86400) is <= 81600)
        {
            sky.color = nightSkyColor;
        }
        else if((time % 86400) is <= 86400)
        {
            sky.color = Color.Lerp(nightSkyColor, daySkyColor, ((time % 86400) - 81600) / 4800);
        }
        #endregion
    }
}
