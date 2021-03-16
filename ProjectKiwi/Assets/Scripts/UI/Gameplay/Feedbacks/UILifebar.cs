using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILifebar : MonoBehaviour
{
    public Image lifebar;
    public Transform targetToFollow;
    public Vector3 positionOffset;

    public float visibleTime;
    private float timeToHide;

    CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (!targetToFollow)
            return;

        transform.LookAt(Camera.main.transform); // TODO: remove camera reference
        transform.position = targetToFollow.transform.position + positionOffset;

        if(Time.time >= timeToHide)
            canvasGroup.alpha = 0;
    }

    public void OnInit(Transform target)
    {
        targetToFollow = target;
        transform.position = targetToFollow.transform.position + positionOffset;
        canvasGroup.alpha = 0;
    }

    public void OnUpdateValue(float lifePercentage)
    {
        lifebar.transform.localScale = new Vector3(lifePercentage, 1, 1);
        canvasGroup.alpha = 1;

        timeToHide = Time.time + visibleTime;
    }

    public void OnReset()
    {
        targetToFollow = null;
        lifebar.transform.localScale = Vector3.one;
        timeToHide = 0;
        canvasGroup.alpha = 0;
    }

    public void OnRemove()
    {
        OnReset();
        SpawnerController.instance.OnPoolingUILifebar(gameObject);
    }
}
