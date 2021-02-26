using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldPlayerPositionIndicator : MonoBehaviour
{
    public Vector3 indicatorLimits;
    public float lerpSpeed = 1f;
    public GameObject visual;

    private void Update()
    {
        Vector3 player = PlayerController.instance.transform.position;
        Vector3 playerRawInput = PlayerController.instance.rawInput;
        Vector3 newPos = new Vector3(indicatorLimits.x * playerRawInput.x, transform.position.y, indicatorLimits.z * playerRawInput.z) + player;
        newPos.y = player.y -0.9f;
        transform.position = Vector3.MoveTowards(transform.position, newPos, lerpSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - player.x) < 0.1f && Mathf.Abs(transform.position.z - player.z) < 0.1f)
            if(visual.activeSelf)
                visual.SetActive(false);
        else
            if(!visual.activeSelf)
                visual.SetActive(true);
    }
}
