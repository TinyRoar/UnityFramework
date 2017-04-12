using UnityEngine;
using System.Collections;
using TinyRoar.Framework;
using UnityEngine.UI;

public sealed class OrbManager : MonoSingleton<OrbManager>
{
    private ObjectPool _objectPool;

    protected override void Awake()
    {
        // store reference to pool for performance reasons
        _objectPool = ObjectPool.Instance;
    }

    public Orb SpawnOrb(Vector3 startPos, Vector3 targetPos)
    {
        // randomize spawn position
        //position += (Vector3) Random.insideUnitCircle * 4;

        GameObject obj = _objectPool.GetInactiveObject("Orb", startPos);

        if (obj == null)
        {
            Debug.LogWarning("Could not get Object out of ObjectPool");
            return null;
        }

        Orb orb = obj.GetComponent<Orb>();
        orb.startPos = startPos;
        orb.targetPos = targetPos;

        return orb;
    }

}
