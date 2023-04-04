using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ObjectPoolItem
{
    public ObjectPoolItem(GameObject objectToPool, int amountToPool, bool shouldExpand)
    {
        _objectToPool = objectToPool;
        _amountToPool = amountToPool;
        _shouldExpand = shouldExpand;
    }

    public GameObject _objectToPool;
    public int _amountToPool;
    public bool _shouldExpand;
}

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler SharedInstance;

    public List<GameObject> _pooledObjects = new List<GameObject>();
    public List<ObjectPoolItem> _itemsToPool = new List<ObjectPoolItem>();

    private void Awake()
    {
        SharedInstance = this;
    }

    public GameObject GetPooledObject(string tag)
    {
        for (int i = 0; i < _pooledObjects.Count; i++)
        {
            if (!_pooledObjects[i].activeInHierarchy && _pooledObjects[i].CompareTag(tag))
            {
                return _pooledObjects[i];
            }
        }

        foreach (ObjectPoolItem item in _itemsToPool)
        {
            if (item._objectToPool.CompareTag(tag))
            {
                if (item._shouldExpand)
                {
                    GameObject obj = (GameObject)Instantiate(item._objectToPool);
                    obj.transform.position = new Vector3(0f, 11f, 0f);
                    obj.SetActive(false);
                    _pooledObjects.Add(obj);
                    return obj;
                }
            }
        }

        return null;
    }

    public void PoolObjects()
    {
        foreach (ObjectPoolItem item in _itemsToPool)
        {
            for (int i = 0; i < item._amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item._objectToPool);
                obj.transform.position = new Vector3(0f, 11f, 0f);
                obj.SetActive(false);
                _pooledObjects.Add(obj);
            }
        }
    }
}