using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectName
{
    hit,
    attack_swing,
    blood,
}

public class EffectManager : MonoBehaviour
{
    public static EffectManager manager;

    [SerializeField] private GameObject[] effects;

    public static GameObject SpawnEffect(EffectName effectName, Vector3 pos, Quaternion rotate, float scale)
    {
        var p = Instantiate(manager.effects[(int)effectName], pos, rotate);
        p.transform.localScale = Vector3.one * scale;

        return p;
    }

    public static GameObject SpawnEffect(EffectName effectName, Vector3 pos, Quaternion rotate, float scale, bool isFlip)
    {
        var p = Instantiate(manager.effects[(int)effectName], pos, rotate);
        p.transform.localScale = Vector3.one * scale;
        
        var localScale = p.transform.localScale;
        localScale.x *= isFlip ? -1 : 1;
        p.transform.localScale = localScale;

        return p;
    }

    public void Awake()
    {
        manager = this;
    }
}
