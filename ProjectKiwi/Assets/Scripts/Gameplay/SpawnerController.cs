using System.Collections;
using System.Collections.Generic;
using Gameplay.Bullets;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    public static SpawnerController instance;

    [Header("Bullets")]
    public GameObject _bullet;

    //Containers
    private GameObject _bulletsContainer;

    //PoolingList
    private Dictionary<string, Queue<GameObject>> _poolList = new Dictionary<string, Queue<GameObject>>();

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public BulletBase OnSpawnBullet(Vector3 newPos, Quaternion newRotation)
    {
        BulletBase bulletBase = null;

        // if (_bulletsContainer == null)
        //     _bulletsContainer = new GameObject("BulletsContainer");
        _bulletsContainer = _bulletsContainer ? _bulletsContainer : new GameObject("BulletsContainer");
        
        //string poolKey = string.Format("bullet {0}", type.ToString());
        var poolKey = string.Format("bullet");
        GameObject go = null;
        if (_poolList.ContainsKey(poolKey) && _poolList[poolKey].Count > 0)
            go = _poolList[poolKey].Dequeue();

        if (go)
        {
            bulletBase = go.GetComponent<BulletBase>();
            var o = bulletBase.gameObject;
            o.transform.position = newPos;
            o.transform.rotation = newRotation;
        }
        else
        {
            bulletBase = Instantiate(_bullet, newPos, newRotation).GetComponent<BulletBase>();
            bulletBase.transform.SetParent(_bulletsContainer.transform);
        }
        bulletBase.gameObject.SetActive(true);
        bulletBase.PoolOnInit();
        return bulletBase;
    }

    #region POOLING
    public void OnPoolingBullets(GameObject go, BulletBase bulletBase)
    {
        //string poolKey = string.Format("bullet {0}", type.ToString());
        var poolKey = string.Format("bullet");
        if (!_poolList.ContainsKey(poolKey))
        {
            Queue<GameObject> newQueue = new Queue<GameObject>();
            _poolList.Add(poolKey, newQueue);
        }
        _poolList[poolKey].Enqueue(go);
        bulletBase.PoolOnDestroy();
        go.SetActive(false);
    }
    #endregion
}
