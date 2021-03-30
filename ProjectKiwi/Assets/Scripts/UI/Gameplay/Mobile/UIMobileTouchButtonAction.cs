using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMobileTouchButtonAction : MonoBehaviour
{
    public void OnSelectAction(int action)
    {
        PlayerController.instance.OnSelectAction((PlayerWeapon)action);
    }
}
