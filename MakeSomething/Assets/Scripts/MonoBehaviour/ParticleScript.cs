using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParticleScript : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float time;

    SpriteRenderer sr;
    private int index;
    private float currentTime;

    public static void SpawnParticle(GameObject particle, Vector3 pos)
    {
        Instantiate(particle, pos, Quaternion.identity);
    }

    void Start()
    {
        if(!TryGetComponent<SpriteRenderer>(out sr))
            sr = gameObject.AddComponent<SpriteRenderer>(); 
    }

    void Update()
    {
        if(sprites.Length > 1)
        {
            currentTime += Time.deltaTime;
        
            if(currentTime > time)
            {
                index++;
                if(index == sprites.Length)
                {
                    Destroy(this.gameObject);
                    return;
                }
                sr.sprite = sprites[index];
                currentTime = 0;
            }
        }
    }
}
