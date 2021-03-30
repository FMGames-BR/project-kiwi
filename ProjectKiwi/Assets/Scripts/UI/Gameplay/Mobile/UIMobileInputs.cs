using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMobileInputs : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
#if UNITY_EDITOR
        gameObject.SetActive(false);
#elif !UNITY_ANDROID && !UNITY_IOS
        gameObject.SetActive(false);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
