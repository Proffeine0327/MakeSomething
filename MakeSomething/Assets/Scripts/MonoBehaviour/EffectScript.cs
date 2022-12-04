using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectScript : MonoBehaviour
{
    [SerializeField] private bool isParticleSystem;
    [SerializeField] private EffectSpriteContainer spriteContainer;
    [SerializeField] private float scale;
    [SerializeField] private float time;

    SpriteRenderer sr;
    private int index;
    private float currentTime;

    void Start()
    {
        if (!TryGetComponent<SpriteRenderer>(out sr))
            sr = gameObject.AddComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!isParticleSystem)
        {
            if (spriteContainer.Sprites.Length > 1)
            {
                currentTime += Time.deltaTime;

                if (currentTime > time)
                {
                    index++;
                    if (index == spriteContainer.Sprites.Length)
                    {
                        Destroy(gameObject);
                        return;
                    }
                    sr.sprite = spriteContainer.Sprites[index];
                    currentTime = 0;
                }
            }
        }
        else
        {
            currentTime += Time.deltaTime;
            if(currentTime > time)
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}
