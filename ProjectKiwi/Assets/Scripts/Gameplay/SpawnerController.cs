using System.Collections;
using System.Collections.Generic;
using Gameplay.Bullets;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    public static SpawnerController instance;

    [Header("Bullets")]
    public GameObject _bullet;
    [Header("UI Lifebar")]
    public GameObject _uiLifebar;

    //Containers
    private GameObject _bulletsContainer, _lifebarContainer;

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

    public UILifebar OnSpawnUILifebar(Transform parent)
    {
        UILifebar lifebar = null;

        // if (_bulletsContainer == null)
        //     _bulletsContainer = new GameObject("BulletsContainer");
        _lifebarContainer = _lifebarContainer ? _lifebarContainer : new GameObject("UI Lifebar");
        if (_lifebarContainer.transform.parent != UICanvasWorldFeedbacks.instance.transform)
        {
            _lifebarContainer.transform.SetParent(UICanvasWorldFeedbacks.instance.transform);
            _lifebarContainer.transform.position = Vector3.zero;
        }

        //string poolKey = string.Format("bullet {0}", type.ToString());
        var poolKey = string.Format("UILifebar");
        GameObject go = null;
        if (_poolList.ContainsKey(poolKey) && _poolList[poolKey].Count > 0)
            go = _poolList[poolKey].Dequeue();

        if (go)
        {
            lifebar = go.GetComponent<UILifebar>();
        }
        else
        {
            lifebar = Instantiate(_uiLifebar).GetComponent<UILifebar>();
            lifebar.transform.SetParent(_lifebarContainer.transform);
        }

        lifebar.gameObject.SetActive(true);
        lifebar.OnInit(parent);
        return lifebar;
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

    public void OnPoolingUILifebar(GameObject go)
    {
        //string poolKey = string.Format("bullet {0}", type.ToString());
        var poolKey = string.Format("UILifebar");
        if (!_poolList.ContainsKey(poolKey))
        {
            Queue<GameObject> newQueue = new Queue<GameObject>();
            _poolList.Add(poolKey, newQueue);
        }
        _poolList[poolKey].Enqueue(go);
        go.SetActive(false);
    }
    #endregion
}
