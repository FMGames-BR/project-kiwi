using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildController : MonoBehaviour
{

    public GameObject placeholder;

   
    private List<GameObject> _obstructions;
    private BuildPlaceholder _bp;
    
    // Start is called before the first frame update
    void Start()
    {
        
        _bp = placeholder.GetComponent <BuildPlaceholder>();
        _obstructions = new List<GameObject>();
    }

    void Update()
    {
        var _t = transform;
        var _newPos = _t.position + _t.forward * 2;
        _newPos.x = Mathf.RoundToInt(_newPos.x);
        _newPos.z = Mathf.RoundToInt(_newPos.z);
	    placeholder.transform.position = _newPos;
    }

    public void resetObstructions()
    {
        _obstructions = new List<GameObject>();
    }
    
    
   
}
