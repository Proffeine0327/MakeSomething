using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{   
    public abstract void Damaged(int amount);
    public abstract void Attack();
}
