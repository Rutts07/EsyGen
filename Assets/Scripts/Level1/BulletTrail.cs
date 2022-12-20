using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    private Vector3 _initialPosition;
    private Vector3 _targetPosition;
    private float _progress;

    [SerializeField] private float _speed = 40f;

    // Start is called before the first frame update
    void Start()
    {
        _initialPosition = new Vector3(transform.position.x, transform.position.y, -1);
    }

    // Update is called once per frame
    void Update()
    {
        _progress = _speed * Time.fixedDeltaTime;
        transform.position = Vector3.Lerp(_initialPosition, _targetPosition, _progress);
    }

    public void SetTargetPosition(Vector2 targetPosition)
    {
        _targetPosition = new Vector3(targetPosition.x, targetPosition.y, -1);
    }
}
