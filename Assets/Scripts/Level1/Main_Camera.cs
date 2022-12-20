using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Camera : MonoBehaviour
{
    private Transform _player;
    // private float _speed = 1.5f;
    private float minX = -7f, maxX = 13f, minY = -4f, maxY = 3f;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Transform>();
        if (_player == null)
            Debug.LogError("Player is null");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Smoothly follow the player
        Vector3 _cameraPosition = new Vector3(_player.position.x, _player.position.y, transform.position.z);
        // transform.position = Vector3.Lerp(transform.position, _cameraPosition, _speed * Time.deltaTime);
        transform.position = _cameraPosition;

        // Restrict Camera movement
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);
    }
}
