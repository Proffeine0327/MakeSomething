using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParticleScript : MonoBehaviour
{
    [SerializeField] private ParticleSpriteContainer spriteContainer;
    [SerializeField] private float scale;
    [SerializeField] private float time;

    SpriteRenderer sr;
    private int index;
    private float currentTime;

    public static void SpawnParticle(GameObject particle, Vector3 pos, Quaternion rotate, float scale)
    {
        var p = MultipleObjectPool.GetObject(particle.name);
        p.transform.position = pos;
        p.transform.rotation = rotate;
        p.transform.localScale = Vector3.one * scale;
    }

    public static void SpawnParticle(GameObject particle, Vector3 pos, Quaternion rotate, float scale, bool isFlip)
    {
        var p = MultipleObjectPool.GetObject(particle.name);
        p.transform.position = pos;
        p.transform.rotation = rotate;
        p.transform.localScale = Vector3.one * scale;
        p.transform.localScale = Vector3.one * scale;
        p.GetComponent<SpriteRenderer>().flipX = isFlip;
    }

    void Start()
    {
        if(!TryGetComponent<SpriteRenderer>(out sr))
            sr = gameObject.AddComponent<SpriteRenderer>(); 
    }

    void Update()
    {
        if(spriteContainer.Sprites.Length > 1)
        {
            currentTime += Time.deltaTime;
        
            if(currentTime > time)
            {
                index++;
                if(index == spriteContainer.Sprites.Length)
                {
                    index = 0;
                    currentTime = 0;
                    MultipleObjectPool.PoolObject(gameObject.name, gameObject);
                    return;
                }
                sr.sprite = spriteContainer.Sprites[index];
                currentTime = 0;
            }
        }
    }
}
