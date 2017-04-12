using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TinyRoar.Framework;
using UnityEngine.UI;

/// <summary>
/// Object Pool Design Pattern
/// by Tiny Roar, 2016
/// </summary>
public class ObjectPool : MonoSingleton<ObjectPool>
{
    [SerializeField]
    private int objectsCount = 50;
    public List<GameObject> prefabs;
    [SerializeField]
    private Transform Container;
    [SerializeField]
    private bool FillPoolInConstructor = true;

    private List<GameObject> _pool;
    private bool _poolReady = false;

    protected override void Awake()
    {
        if(FillPoolInConstructor)
            FillPool();
    }

    public void FillPool()
    {
        if (_poolReady)
        {
            Debug.LogWarning("Pool already filled :'(");
            return;
        }

        _pool = new List<GameObject>();

        var count = prefabs.Count;
        for (var i = 0; i < count; i++)
        {
            for (var o = 0; o < objectsCount; o++)
            {
                SpawnObject(prefabs[i]);
            }
        }

        _poolReady = true;
    }

    public void DestroyAll()
    {
        var count = _pool.Count;
        for (var i = 0; i < count; i++)
        {
            GameObject item = _pool[i];
            if (item.activeSelf == true)
            {
                item.SetActive(false);
            }
        }
    }

    public void Deactivate(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    public GameObject GetInactiveObject(string name)
    {
        if (!_poolReady)
        {
            Debug.LogWarning("Pool not ready yet :'(");
            return null;
        }

        // get object out of pool
        var count = _pool.Count;
        for (var i = 0; i < count; i++)
        {
            GameObject item = _pool[i];
            if (item.activeSelf == false && string.CompareOrdinal(item.name, name) == 0)
            {

                item.SetActive(true);
                return item;
            }
        }

        // fallback if no objects available
        var prefab = GetPrefabWithName(name);
        var obj = SpawnObject(prefab);
        obj.SetActive(true);
        return obj;
    }

    private GameObject SpawnObject(GameObject prefab)
    {
        var obj = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
        obj.transform.SetParent(Container, false);
        obj.name = prefab.name;
        obj.SetActive(false);
        _pool.Add(obj);
        return obj;
    }

    private GameObject GetPrefabWithName(string name)
    {
        for (var i = 0; i < prefabs.Count; i++)
        {
            var prefab = prefabs[i];
            if (prefab.name == name)
                return prefab;
        }
        return null;
    }

    public void SpawnParticleEffect(string prefabName, Vector2 pos)
    {
        StartCoroutine(ParticleEffect(prefabName, pos));
    }

    public IEnumerator ParticleEffect(string prefabName, Vector2 pos)
    {
        var go = ObjectPool.Instance.GetInactiveObject(prefabName);
        go.transform.position = pos;
        var particle = go.GetComponentInChildren<ParticleSystem>();

        // if particle if effect has loop stop here
        if (particle.main.loop)
            yield break;

        while (particle.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }

        ObjectPool.Instance.Deactivate(go);

        yield return null;
    }

    public int CountActiveObjects()
    {
        var count = 0;
        var poolCount = _pool.Count;
        for (var i = 0; i < poolCount; i++)
        {
            if (_pool[i].activeSelf)
                count++;
        }
        return count;
    }


}
