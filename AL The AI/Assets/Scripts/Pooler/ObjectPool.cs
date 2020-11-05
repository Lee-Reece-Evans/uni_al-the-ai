using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string poolTag;
        public GameObject prefab;
        public int size;
    }

    public static ObjectPool Instance;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public List<Pool> pools;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools) // initialise disctionary references to object pools
        {
            Queue<GameObject> objPool = new Queue<GameObject>();
            pool.prefab.GetComponent<IPooledObject>().SetPoolDetails(pool.poolTag); // set tag in prefab so that it can be returned to dictionary

            for (int i = 0; i < pool.size; i++)
            {
                GameObject objToPool = Instantiate(pool.prefab);
                objToPool.SetActive(false);
                objPool.Enqueue(objToPool); // put GO into queue
            }

            poolDictionary.Add(pool.poolTag, objPool); // make dictionary entry
        }
    }

    public GameObject SpawnFromPool(string tag) // retrieve an object from a pool
    {
        if (!poolDictionary.ContainsKey(tag))
            return null;

        if (poolDictionary[tag].Count == 0)
            AddPooledObject(tag, 1); // if queue is empty add 1

        return poolDictionary[tag].Dequeue(); // take GO out of queue
    }

    public void AddPooledObject(string tag, int amount) // add an object to the pool if there isn't one available
    {
        int index = pools.FindIndex(i => i.poolTag == tag);

        GameObject objToPool = Instantiate(pools[index].prefab);
        objToPool.SetActive(false);
        poolDictionary[tag].Enqueue(objToPool); // put GO into queue
    }

    public void ReturnToPool(string tag, GameObject returnedObject) // return an object back to the pool
    {
        returnedObject.SetActive(false);
        poolDictionary[tag].Enqueue(returnedObject);
    }
}
