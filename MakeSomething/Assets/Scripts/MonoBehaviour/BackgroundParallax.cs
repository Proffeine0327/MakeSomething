using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    [SerializeField] private float xPosParallaxRatio;
    private Vector3 center;
    
    private void Start() 
    {
        center = transform.position;
    }

    private void Update() 
    {
        float dis = Camera.main.transform.position.x * xPosParallaxRatio;
        transform.position = new Vector3(center.x + dis, Camera.main.transform.position.y - 1.5f, transform.position.z);
    }
}
