using System.Collections;
using System.Collections.Generic;
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

    public GameObject OnSpawnBullet()
    {
        GameObject bullet = null;

        if (_bulletsContainer == null)
            _bulletsContainer = new GameObject("BulletsContainer");

        //string poolKey = string.Format("bullet {0}", type.ToString());
        string poolKey = string.Format("bullet");
        GameObject go = null;
        if (_poolList.ContainsKey(poolKey) && _poolList[poolKey].Count > 0)
            go = _poolList[poolKey].Dequeue();

        if (go == null)
        {
            //switch (type)
            //{
            //    case Towers.A:
            //        bullet = Instantiate(_towerA, _bulletsContainer.transform);
            //        break;
            //    case Towers.B:
            //        bullet = Instantiate(_towerB, _bulletsContainer.transform);
            //        break;
            //    case Towers.C:
            //        bullet = Instantiate(_towerC, _bulletsContainer.transform);
            //        break;
            //    case Towers.D:
            //        bullet = Instantiate(_towerD, _bulletsContainer.transform);
            //        break;
            //}

            bullet = Instantiate(_bullet, _bulletsContainer.transform);
        }
        else
            bullet = go;

        bullet.gameObject.SetActive(true);
        //bullet.OnSpawn();

        return bullet;
    }

    #region POOLING

    public void OnPoolingBullets(GameObject bullet)
    {
        //string poolKey = string.Format("bullet {0}", type.ToString());
        string poolKey = string.Format("bullet");

        if (!_poolList.ContainsKey(poolKey))
        {
            Queue<GameObject> newQueue = new Queue<GameObject>();
            _poolList.Add(poolKey, newQueue);
        }

        _poolList[poolKey].Enqueue(bullet);

        bullet.SetActive(false);
        //bullet.OnReset();
    }

    #endregion
}
