using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{   
    [SerializeField] protected int maxHp;
    [SerializeField] protected int currentHp;

    public int MaxHp { get { return maxHp; } }
    public int CurrentHp { get { return currentHp; } }
    
    public abstract void Damaged(int amount);
    public abstract void Attack();
}
