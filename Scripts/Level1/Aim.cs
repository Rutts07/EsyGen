using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    private Transform _aimTransform;
    
    private Vector2 _position;
    private Vector2 _mousePosition; 
    private Vector2 _direction;
    private float _angle;

    [SerializeField] private Camera cam;

    void Awake() 
    {
        _aimTransform = transform.Find("Aim");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        _position = new Vector2(transform.position.x, transform.position.y);
        _direction = (_mousePosition - _position).normalized;
        _angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

        _aimTransform.rotation = Quaternion.Euler(0, 0, _angle);
        
    }
}
