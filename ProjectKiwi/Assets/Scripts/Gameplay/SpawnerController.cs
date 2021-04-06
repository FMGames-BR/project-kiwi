using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Enums;
using Gameplay.Bullets;
using Gameplay.Weapons;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    public static SpawnerController instance;

    [Header("Bullets")]
    public GameObject _bullet;
    [Header("UI Lifebar")]
    public GameObject _uiLifebar;
    [Header("Weapons")]
    public GameObject _shotgun;
    public GameObject _grenadeLauncher;
    public GameObject _bazooka;

    //Containers
    private GameObject _weaponsContainer, _bulletsContainer, _lifebarContainer;

    //PoolingList
    private Dictionary<string, Queue<GameObject>> _poolList = new Dictionary<string, Queue<GameObject>>();

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }
    public WeaponBase OnSpawnWeapon(Weapon weaponType, Transform weaponParent)
    {
        WeaponBase weaponBase = null;

        _weaponsContainer = _weaponsContainer ? _weaponsContainer : new GameObject("WeaponsContainer");

        var poolKey = string.Format("weapon {0}", weaponType.ToString());
        GameObject go = null;
        if (_poolList.ContainsKey(poolKey) && _poolList[poolKey].Count > 0)
            go = _poolList[poolKey].Dequeue();

        if (go)
        {
            weaponBase = go.GetComponent<WeaponBase>();
            weaponBase.transform.SetParent(weaponParent);
            weaponBase.transform.localPosition = Vector3.zero;
            weaponBase.transform.transform.localEulerAngles = Vector3.zero;
        }
        else
        {
            GameObject objToSpawn = null;

            switch (weaponType)
            {
                case Weapon.Shotgun:
                    objToSpawn = _shotgun;
                    break;
                case Weapon.Grenade:
                    objToSpawn = _grenadeLauncher;
                    break;
                case Weapon.Bazooka:
                    objToSpawn = _bazooka;
                    break;
            }

            weaponBase = Instantiate(objToSpawn, weaponParent).GetComponent<WeaponBase>();
        }
        weaponBase.gameObject.SetActive(true);
        return weaponBase;
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
    public void OnPoolingWeapons(WeaponBase weapon)
    {
        var poolKey = string.Format("weapon {0}", weapon.data.type.ToString());
        if (!_poolList.ContainsKey(poolKey))
        {
            Queue<GameObject> newQueue = new Queue<GameObject>();
            _poolList.Add(poolKey, newQueue);
        }
        _poolList[poolKey].Enqueue(weapon.gameObject);

        //extract from player and leave on container
        weapon.transform.SetParent(_weaponsContainer.transform);

        weapon.gameObject.SetActive(false);
    }

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
