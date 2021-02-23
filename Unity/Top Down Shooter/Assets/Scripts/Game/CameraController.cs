using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float smooth = 0.5f;
    public float distance = 4f;
    public float height = 14;
  
    private Transform _player;
    private Vector3 _velocity = Vector3.zero;
    private float _forcedXRotation = 70;
    private void Start()
    {
        _player = GameObject.FindWithTag("Player").transform;
        transform.eulerAngles = new Vector3(_forcedXRotation, 0, 0);
    }

    private void Update()
    {
        var playerPosition = _player.position;
        var target = new Vector3(playerPosition.x, playerPosition.y + height, playerPosition.z - distance);
        transform.position = Vector3.SmoothDamp(transform.position, target, ref _velocity, smooth);
    }
    
    
}
    
    
 