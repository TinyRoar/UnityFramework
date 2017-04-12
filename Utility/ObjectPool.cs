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

    private List<GameObject> pool;
    private bool poolReady = false;

    protected override void Awake()
    {
        FillPool();
    }

    public void FillPool()
    {
        pool = new List<GameObject>();

        for (var i = 0; i < prefabs.Count; i++)
        {
            for (var o = 0; o < objectsCount; o++)
            {
                SpawnObject(prefabs[i]);
            }
        }

        poolReady = true;
    }

    public void DestroyAll()
    {
        var count = pool.Count;
        for (var i = 0; i < count; i++)
        {
            GameObject item = pool[i];
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

    public GameObject GetInactiveObject(string name, Vector3 spawnPosition)
    {
        // get object out of pool
        var count = pool.Count;
        for (var i = 0; i < count; i++)
        {
            GameObject item = pool[i];
            if (item.activeSelf == false && string.CompareOrdinal(item.name, name) == 0)
            {

                item.SetActive(true);
                item.transform.position = spawnPosition;
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
        GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
        obj.transform.SetParent(Container);
        obj.name = prefab.name;
        obj.SetActive(false);
        pool.Add(obj);
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
        var go = ObjectPool.Instance.GetInactiveObject(prefabName, new Vector3(pos.x, pos.y));
        var particle = go.GetComponentInChildren<ParticleSystem>();

        // if particle if effect has loop stop here
        if (particle.main.loop)
            yield break;

        while (particle.isPlaying)
        {
            yield return new WaitForSeconds(1f / 60f);
        }

        ObjectPool.Instance.Deactivate(go);

        yield return null;
    }


}
