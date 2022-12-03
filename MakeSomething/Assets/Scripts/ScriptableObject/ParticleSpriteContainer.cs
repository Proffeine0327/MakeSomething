using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Particle", fileName = "Particle_")]
public class ParticleSpriteContainer : ScriptableObject
{
    [SerializeField] private Sprite[] sprites;

    public Sprite[] Sprites { get { return sprites; } }
}
