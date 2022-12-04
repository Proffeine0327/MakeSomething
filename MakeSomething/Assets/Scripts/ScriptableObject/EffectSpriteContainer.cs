using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Effect", fileName = "Effect_")]
public class EffectSpriteContainer : ScriptableObject
{
    [SerializeField] private Sprite[] sprites;

    public Sprite[] Sprites { get { return sprites; } }
}
