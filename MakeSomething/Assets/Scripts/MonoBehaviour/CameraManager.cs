using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager manager { get; set; }

    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float followSpeed;
    
    void FixedUpdate()
    {
        if(target != null)
        {
            var vector2Transform = target.transform.position;
            vector2Transform.z = transform.position.z;                      
            transform.position = Vector3.Lerp(transform.position, vector2Transform + offset, followSpeed);
        }
    }
}
