using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    [SerializeField] private Vector2 parallax;
    [SerializeField] private float length;
    private Vector3 center;

    private void Start()
    {
        center = Camera.main.transform.position;
    }

    private void Update() 
    {
        float temp = (Camera.main.transform.position.x - transform.position.x);
        var dist = (Camera.main.transform.position - center) * parallax;

        transform.position = new Vector3(center.x + dist.x, center.y + dist.y, transform.position.z);
        if(Mathf.Abs(temp) > length) center.x = Camera.main.transform.position.x;
    }
}
