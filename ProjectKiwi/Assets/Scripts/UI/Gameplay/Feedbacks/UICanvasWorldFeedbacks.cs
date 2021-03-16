using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvasWorldFeedbacks : MonoBehaviour
{
    public static UICanvasWorldFeedbacks instance;

    private void Awake()
    {
        instance = this;
    }
}
