using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager manager;

    [SerializeField] private float gameTime;
    
    private void Awake() 
    {
        manager = this;
    }

    void Update()
    {
        //1sec == 1 game min;
        //8640sec == 2.4hour == 1 game day
        gameTime += Time.deltaTime * 60;
        DayPass.SetTime(gameTime);
    }
}
