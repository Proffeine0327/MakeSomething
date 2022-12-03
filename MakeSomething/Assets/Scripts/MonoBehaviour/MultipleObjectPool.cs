using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPoolInfo
{
    [SerializeField] private GameObject poolObject;
    [SerializeField] private int maxAmount;

    public int MaxAmount { get { return maxAmount; } }
    public GameObject PoolObject { get { return poolObject; } }
}

public class ObjectPool
{
    public int maxAmount;
    public GameObject poolObject;
    public Queue<GameObject> pool;
}

public class MultipleObjectPool : MonoBehaviour
{
    private static MultipleObjectPool currentObjectPool;

    [SerializeField] private ObjectPoolInfo[] infos;
    Dictionary<string, ObjectPool> table;

    public static GameObject GetObject(string key)
    {
        if(!currentObjectPool.table.ContainsKey(key))
            throw new ArgumentNullException($"Pooling table not contains \"{key}\"");
        
        if(currentObjectPool.table[key].pool.Count > 0)
        {
            var getObj = currentObjectPool.table[key].pool.Dequeue();
            getObj.SetActive(true);
            getObj.transform.SetParent(null);
            return getObj;
        }
        else
        {
            var newObj = Instantiate(currentObjectPool.table[key].poolObject, Vector3.zero, Quaternion.identity);
            newObj.name = newObj.name.Substring(0,newObj.name.Length - 7);
            newObj.SetActive(true);
            newObj.transform.SetParent(null);
            
            return newObj;
        }
    }

    public static void PoolObject(string key, GameObject obj)
    {
        if(!currentObjectPool.table.ContainsKey(key))
            throw new ArgumentNullException($"Pooling table not contains \"{key}\"");

        if(currentObjectPool.table[key].pool.Count >= currentObjectPool.table[key].maxAmount)
        {
            Destroy(obj);
        }
        else
        {
            obj.SetActive(false);
            obj.transform.SetParent(currentObjectPool.transform.Find($"Pool \"{key}\"").transform);
            currentObjectPool.table[key].pool.Enqueue(obj);
        }
    }
    
    private void Awake() 
    {
        currentObjectPool = this;
        table = new Dictionary<string, ObjectPool>();

        foreach(var info in infos)
        {
            var poolGroup = new GameObject($"Pool \"{info.PoolObject.name}\"");
            poolGroup.transform.SetParent(transform);

            var q = new Queue<GameObject>();
            for(int i = 0; i < info.MaxAmount; i++) 
            {
                var newObj = Instantiate(info.PoolObject, Vector3.zero, Quaternion.identity, poolGroup.transform);
                newObj.name = newObj.name.Substring(0,newObj.name.Length - 7);
                newObj.SetActive(false);

                q.Enqueue(newObj);
            }

            var newObjPool = new ObjectPool();
            newObjPool.pool = q;
            newObjPool.maxAmount = info.MaxAmount;
            newObjPool.poolObject = info.PoolObject;

            table.Add(info.PoolObject.name, newObjPool);
        }
    }
}
