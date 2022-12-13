using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayPass : MonoBehaviour
{
    private static DayPass daypass;

    /// <summary>
    /// Set sun and moon group rotation, and background color.
    /// </summary>
    /// <param name="t">Time to set. 1t == 1min(1/3600day)</param>
    public static void SetTime(float t)
    {
        daypass.transform.rotation = Quaternion.Euler(0,0,-t/10);
    }

    [SerializeField] private GameObject sun;
    [SerializeField] private GameObject moon;
    [SerializeField] private SpriteRenderer sky;

    private void Awake() 
    {
        daypass = this;    
        StartCoroutine(test());
    }

    IEnumerator test()
    {
        yield return new WaitForSeconds(1);
        for(int i = 0; i < 3600; i++)
        {
            SetTime(i);
            yield return new WaitForEndOfFrame();
        }
    }

    void Update()
    {
        sun.transform.rotation = Quaternion.Euler(0,0,0);
        moon.transform.rotation = Quaternion.Euler(0,0,0);
    }
}
